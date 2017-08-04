using System;

using Android.App;
using Android.OS;

namespace AssessmentWeatherApp
{
    [Activity(Label = "Credits", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Credits : Activity
    {
        int currentHour;
        //string nightTime = "";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // determining hour of day, to display either day/night or dusk/dawn background
            currentHour = DateTime.Now.Hour;

            // used this site to determine dusk and dawn --http://rasnz.org.nz/in-the-sky/sun-rise-and-set

            // 8am up to and including 5pm
            if (currentHour >= 8 && currentHour < 18)
            {
                SetContentView(Resource.Layout.creditsDay);
            }
            // 9pm up to and including 11pm or 12pm up to and including 4am
            else if (currentHour >= 20 && currentHour > 0 || currentHour >= 0 && currentHour < 5)
            {
                //nightTime = "night";
                SetContentView(Resource.Layout.creditsNight);
            }
            // 5am up to and including 7am or 6pm up to and including 8pm
            else if (currentHour >= 5 && currentHour < 8 || currentHour >= 18 && currentHour < 20)
            {
                SetContentView(Resource.Layout.creditsDuskDawn);
            }
        }
    }
}