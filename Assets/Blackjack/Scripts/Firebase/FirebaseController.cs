using UnityEngine;
using Firebase.Extensions;
using Firebase;
using Firebase.Crashlytics;

public class FirebaseController : MonoBehaviour
{

    private FirebaseApp app;

    public static FirebaseController instance;

    public static bool isFirebaseInit;

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
            string logEvent = $"ID => {SystemInfo.deviceUniqueIdentifier } || Application Version => { Application.version  }  || FunctionName {functionName} || Log => {log} ";
            Debug.Log($"scriptName : { scriptName } FirelogEvent { logEvent } log : { log }");
            Firebase.Analytics.FirebaseAnalytics.LogEvent(scriptName, logEvent, log);
        }
    }

    public void FireCrash(string scriptName, string functionName, string reason)
    {
        if (isFirebaseInit)
        {
            string exception = $"ID => {SystemInfo.deviceUniqueIdentifier } || Application Version => { Application.version  }  || FunctionName {functionName} || Exception => {reason} ";
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

                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    // Set the log level for Firebase Analytics
                    FirelogEvent("FirebaseController", "FireBaseInitialize", "Firebase Successful Initialized");
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
        throwExceptionEvery60Updates();
    }

    // A method that tests your Crashlytics implementation by throwing an
    // exception every 60 frame updates. You should see reports in the
    // Firebase console a few minutes after running your app with this method.
    void throwExceptionEvery60Updates()
    {
        if (isFirebaseInit)
            if (updatesBeforeException > 0)
            {
                //updatesBeforeException--;
            }
            else
            {
                updatesBeforeException++;
                // Set the counter to 60 updates
                //updatesBeforeException = 0;

                string exception = $"ID => {SystemInfo.deviceUniqueIdentifier } || Application Version => { Application.version  }  || FunctionName throwExceptionEvery60Updates || Exception => test exception please ignore ";
                Debug.Log($"<color><b> Exception  { exception }</b></color>");
                FireCrash("FirebaseController", "throwExceptionEvery60Updates", "test exception please ignore");
                //Crashlytics.SetCustomKey("FirebaseController", exception);

                // Throw an exception to test your Crashlytics implementation
                throw new System.Exception("test exception please ignore");
            }
    }
}
