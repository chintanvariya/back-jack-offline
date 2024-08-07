using UnityEngine;
using Firebase.Extensions;
using Firebase;
using Firebase.Crashlytics;
using Firebase.RemoteConfig;
using System.Threading.Tasks;
using System;
using BlackJackOffline;
using static BlackJackOffline.BlackJackRemoteConfigClass;


public class FirebaseController : MonoBehaviour
{

    private FirebaseApp app;

    public static FirebaseController instance;

    public static bool isFirebaseInit;

    public BlackJackGoogleAdmobManage googleMobileAdsController;

    public static System.Action<string, string, string> CallFireBaseEvent;
    public static System.Action<string, string, string> CallFireBaseCrash;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //Debug.Log("json:" + JsonUtility.ToJson(remoteConfigData));
        //CheckRemoteConfigValues();
    }

    public BlackJackRemoteConfig remoteConfigData;

    public Task CheckRemoteConfigValues()
    {
        Debug.Log("Fetching data...");
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task =>
            {
                Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");

                string configData = remoteConfig.GetValue("Black_Jack_21_Remote_Config").StringValue;
                Debug.Log($"<color><b>BlackJackRemoteConfig => </b>{configData}</color>");
                remoteConfigData = JsonUtility.FromJson<BlackJackRemoteConfig>(configData);

                if (remoteConfigData.flagDetails.isAds)
                    googleMobileAdsController.InitializeAds();

                /*  print("Total values: "+remoteConfig.AllValues.Count);

                  foreach (var item in remoteConfig.AllValues)
                  {
                      print("Key :" + item.Key);
                      print("Value: " + item.Value.StringValue);
                  }*/

            });
    }

    private void Start()
    {
        FireBaseInitialize();
        updatesBeforeException = 0;
    }

    private void OnEnable()
    {
        CallFireBaseEvent += FirelogEvent;
        CallFireBaseCrash += FireCrash;
    }

    private void OnDisable()
    {
        CallFireBaseEvent -= FirelogEvent;
        CallFireBaseCrash -= FireCrash;
    }


    public void FirelogEvent(string scriptName, string functionName, string log)
    {
        if (isFirebaseInit)
        {
            string logEvent = $"ID_{SystemInfo.deviceUniqueIdentifier}_Application_Version_{Application.version}_FunctionName_{functionName}_Log_{log}";
            Debug.Log($"$LogEVENT {logEvent}");
            Firebase.Analytics.FirebaseAnalytics.LogEvent(scriptName, functionName, log);
        }
    }

    public void FireCrash(string scriptName, string functionName, string reason)
    {
        if (isFirebaseInit)
        {
            string exception = $"ID_{SystemInfo.deviceUniqueIdentifier}_Application_Version_{Application.version}_FunctionName_{functionName}_Exception_{reason}";
            Crashlytics.SetCustomKey(scriptName, exception);
        }
    }

    public void FireBaseInitialize()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {

                app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log("Firebase Successful Init");

                isFirebaseInit = true;

                CheckRemoteConfigValues();

                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    // Set the log level for Firebase Analytics
                    FirelogEvent("FirebaseController", "FireBaseInitialize", "Firebase_Successful_Initialized");
                });

                //FireBaseInitRemote.instance.DataFetch();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }


    // Start is called before the first frame update
    int updatesBeforeException;

    // Update is called once per frame
    void Update()
    {
        // Call the exception-throwing method here so that it's run
        // every frame update
        //throwExceptionEvery60Updates();
    }

    // A method that tests your Crashlytics implementation by throwing an
    // exception every 60 frame updates. You should see reports in the
    // Firebase console a few minutes after running your app with this method.
    void throwExceptionEvery60Updates()
    {
        if (updatesBeforeException > 0)
        {
            updatesBeforeException--;
        }
        else
        {
            // Set the counter to 60 updates
            updatesBeforeException = 0;

            //string exception = $"ID => {SystemInfo.deviceUniqueIdentifier } || Application Version => { Application.version  }  || FunctionName throwExceptionEvery60Updates || Exception => test exception please ignore ";
            FireCrash("FirebaseController", "throwExceptionEvery60Updates", "test exception please ignore");
            //Crashlytics.SetCustomKey("FirebaseController", exception);

            // Throw an exception to test your Crashlytics implementation
            throw new System.Exception("test exception please ignore");
        }
    }
}
