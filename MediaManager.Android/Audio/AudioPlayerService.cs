using System;
using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;

namespace Plugin.MediaManager.Audio
{
    [Service(Exported = true)]
    [IntentFilter(new[] { ServiceInterface })]
    public class AudioPlayerService : MediaBrowserServiceCompat
    {
        public AudioPlayerService()
        {
            Debugger.Break();
        }

        protected AudioPlayerService(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Debugger.Break();
        }

        protected MediaSessionCompat _mediaSession;
        public MediaSessionCallback MediaSessionCallback { get; private set; }
        public AudioPlayback AudioPlayback { get; set; }

        public override void OnCreate()
        {
            base.OnCreate();

            InitMediaPlayer();
            InitMediaSession();

            // This is an Intent to launch the app's UI, used primarily by the ongoing notification.
            //Intent intent = new Intent(context, GetType());
            //intent.AddFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
            //PendingIntent pi = PendingIntent.GetActivity(context, REQUEST_CODE, intent,
            //        PendingIntentFlags.UpdateCurrent);
            //mSession.SetSessionActivity(pi);
        }

        public void InitMediaPlayer()
        {
            AudioPlayback = new AudioPlayback();
        }

        public void InitMediaSession()
        {
            // Start a new MediaSession.
            _mediaSession = new MediaSessionCompat(this, this.GetType().Name);
            SessionToken = _mediaSession.SessionToken;
            MediaSessionCallback = new MediaSessionCallback(this);
            _mediaSession.SetCallback(MediaSessionCallback);
            _mediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons |
                                   MediaSessionCompat.FlagHandlesTransportControls);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            MediaButtonReceiver.HandleIntent(_mediaSession, intent);
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            _mediaSession.Release();
            base.OnDestroy();
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            return new BrowserRoot(nameof(ApplicationContext.ApplicationInfo.Name), // Name visible in Android Auto
                 null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            result.SendResult(null);
        }

        public override void OnCustomAction(string action, Bundle extras, MediaBrowserServiceCompat.Result result)
        {
            base.OnCustomAction(action, extras, result);
        }
    }
}