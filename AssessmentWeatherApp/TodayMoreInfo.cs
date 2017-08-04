using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace AssessmentWeatherApp
{
    [Activity(Label = "Today's Weather", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TodayMoreInfo : Activity
    {
        // layout variables
        TextView txtCityName;
        ImageView imgCondition;
        TextView txtTemp;
        TextView txtCondition;
        TextView txtDate;
        TextView txtMaxMin;
        TextView txtWindDirection;
        TextView txtWindSpeed;
        TextView txtWindGustSpeed;
        TextView txtFeelsLike;
        TextView txtHumidity;
        TextView txtObservationTime;

        // programming variables
        int currentHour;
        string nightTime = "";
        string date;
        string cityName;
        string image;
        string temp;
        string condition;
        string maxMin;
        string windDirection;
        string windSpeed;
        string windGustSpeed;
        string feelsLike;
        string humidity;
        string observationTime;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // determining hour of day, to display either day/night or dusk/dawn background
            currentHour = DateTime.Now.Hour;            

            // used this site to determine dusk and dawn --http://rasnz.org.nz/in-the-sky/sun-rise-and-set

            // 8am up to and including 5pm
            if (currentHour >= 8 && currentHour < 18)
            {
                SetContentView(Resource.Layout.moreInfoDay);
            }
            // 9pm up to and including 11pm or 12pm up to and including 4am
            else if (currentHour >= 20 && currentHour > 0 || currentHour >= 0 && currentHour < 5)
            {
                nightTime = "night";
                SetContentView(Resource.Layout.moreInfoNight);
            }
            // 5am up to and including 7am or 6pm up to and including 8pm
            else if (currentHour >= 5 && currentHour < 8 || currentHour >= 18 && currentHour < 20)
            {
                SetContentView(Resource.Layout.moreInfoDuskDawn);
            }

            // attach events to layout resources
            txtCityName = FindViewById<TextView>(Resource.Id.txtCityName);
            imgCondition = FindViewById<ImageView>(Resource.Id.imgCondition);
            txtTemp = FindViewById<TextView>(Resource.Id.txtTemp);
            txtCondition = FindViewById<TextView>(Resource.Id.txtCondition);            
            txtMaxMin = FindViewById<TextView>(Resource.Id.txtMaxMin);
            txtDate = FindViewById<TextView>(Resource.Id.txtDate);
            txtWindDirection = FindViewById<TextView>(Resource.Id.txtWindDirection);
            txtWindSpeed = FindViewById<TextView>(Resource.Id.txtWindSpeed);
            txtWindGustSpeed = FindViewById<TextView>(Resource.Id.txtWindGustSpeed);
            txtFeelsLike = FindViewById<TextView>(Resource.Id.txtFeelsLike);
            txtHumidity = FindViewById<TextView>(Resource.Id.txtHumidity);
            txtObservationTime = FindViewById<TextView>(Resource.Id.txtObservationTime);            

            DisplayMoreInfo();                       
        }

        private void DisplayMoreInfo()
        {
            cityName = Intent.GetStringExtra("CityName");
            image = Intent.GetStringExtra("Image");

            if (nightTime != "night")
            {
                GetWeatherImageTodayDay(image); // day images, includes dusk and dawn
            }
            else
            {
                GetWeatherImageTodayNight(image); // night images
            }

            temp = Intent.GetStringExtra("Temp");
            condition = Intent.GetStringExtra("Condition");
            maxMin = Intent.GetStringExtra("MaxMinTemp");
            date = Intent.GetStringExtra("Date");
            windDirection = Intent.GetStringExtra("WindDirection");
            windSpeed = Intent.GetStringExtra("WindSpeed");
            windGustSpeed = Intent.GetStringExtra("WindGustSpeed");
            feelsLike = Intent.GetStringExtra("FeelsLike");
            humidity = Intent.GetStringExtra("Humidity");
            observationTime = Intent.GetStringExtra("ObservationTime");

            txtCityName.Text = cityName;
            // image
            txtTemp.Text = temp;
            txtCondition.Text = condition;
            txtMaxMin.Text = maxMin;
            txtDate.Text = date;
            txtWindDirection.Text = "Direction - " + windDirection;
            txtWindSpeed.Text = "Speed - " + windSpeed + " kph";
            txtWindGustSpeed.Text = "Gusting at " + windGustSpeed + " kph";
            txtFeelsLike.Text = feelsLike + " °C";
            txtHumidity.Text = humidity;
            txtObservationTime.Text = observationTime;
        }

        public void GetWeatherImageTodayDay(string imgicon) // determining and setting day image for today's weather
        {                                                   // dusk and dawn use day images too
            if (imgicon == "nodata")
            {
                imgCondition.SetImageResource(Resource.Drawable.nodata);
            }
            else
            {
                if (imgicon == "chanceflurries" || imgicon == "flurries")
                {
                    imgCondition.SetImageResource(Resource.Drawable.flurries);
                }
                else if (imgicon == "chancerain")
                {
                    imgCondition.SetImageResource(Resource.Drawable.chancerain);
                }
                else if (imgicon == "chancesleet" || imgicon == "sleet")
                {
                    imgCondition.SetImageResource(Resource.Drawable.sleet);
                }
                else if (imgicon == "chancesnow")
                {
                    imgCondition.SetImageResource(Resource.Drawable.chancesnow);
                }
                else if (imgicon == "chancetstorms")
                {
                    imgCondition.SetImageResource(Resource.Drawable.chancetstorms);
                }
                else if (imgicon == "clear" || imgicon == "sunny")
                {
                    imgCondition.SetImageResource(Resource.Drawable.sunny);
                }
                else if (imgicon == "cloudy")
                {
                    imgCondition.SetImageResource(Resource.Drawable.cloudy);
                }
                else if (imgicon == "fog")
                {
                    imgCondition.SetImageResource(Resource.Drawable.fog);
                }
                else if (imgicon == "hazy")
                {
                    imgCondition.SetImageResource(Resource.Drawable.haze);
                }
                else if (imgicon == "mostlycloudy" || imgicon == "partlysunny")
                {
                    imgCondition.SetImageResource(Resource.Drawable.mostcloudy);
                }
                else if (imgicon == "partlycloudy" || imgicon == "mostlysunny")
                {
                    imgCondition.SetImageResource(Resource.Drawable.partcloudy);
                }
                else if (imgicon == "rain")
                {
                    imgCondition.SetImageResource(Resource.Drawable.rain);
                }
                else if (imgicon == "snow")
                {
                    imgCondition.SetImageResource(Resource.Drawable.snow);
                }
                else if (imgicon == "tstorms")
                {
                    imgCondition.SetImageResource(Resource.Drawable.tstorms);
                }
                else if (imgicon == "unknown" || imgicon == "")
                {
                    imgCondition.SetImageResource(Resource.Drawable.unknown);
                }
            }
        }

        public void GetWeatherImageTodayNight(string imgicon) // determining and setting night image for today's weather
        {
            if (imgicon == "nodata")
            {
                imgCondition.SetImageResource(Resource.Drawable.nodata);
            }
            else
            {
                if (imgicon == "chanceflurries" || imgicon == "chancesnow" || imgicon == "flurries" || imgicon == "snow")
                {
                    imgCondition.SetImageResource(Resource.Drawable.nsnow);
                }
                else if (imgicon == "chancerain" || imgicon == "rain")
                {
                    imgCondition.SetImageResource(Resource.Drawable.nrain);
                }
                else if (imgicon == "chancesleet" || imgicon == "sleet")
                {
                    imgCondition.SetImageResource(Resource.Drawable.sleet);
                }
                else if (imgicon == "chancetstorms" || imgicon == "tstorms")
                {
                    imgCondition.SetImageResource(Resource.Drawable.ntstorms);
                }
                else if (imgicon == "clear")
                {
                    imgCondition.SetImageResource(Resource.Drawable.nclear);
                }
                else if (imgicon == "cloudy")
                {
                    imgCondition.SetImageResource(Resource.Drawable.cloudy);
                }
                else if (imgicon == "fog")
                {
                    imgCondition.SetImageResource(Resource.Drawable.fog);
                }
                else if (imgicon == "hazy")
                {
                    imgCondition.SetImageResource(Resource.Drawable.haze);
                }
                else if (imgicon == "mostlycloudy")
                {
                    imgCondition.SetImageResource(Resource.Drawable.nmostcloudy);
                }
                else if (imgicon == "partlycloudy")
                {
                    imgCondition.SetImageResource(Resource.Drawable.npartcloudy);
                }
                else if (imgicon == "unknown" || imgicon == "")
                {
                    imgCondition.SetImageResource(Resource.Drawable.unknown);
                }
                else if (imgicon == "mostlysunny") // in case sunny conditions exist at night in the api response
                {
                    imgCondition.SetImageResource(Resource.Drawable.partcloudy);
                }
                else if (imgicon == "partlysunny") // in case sunny conditions exist at night in the api response
                {
                    imgCondition.SetImageResource(Resource.Drawable.mostcloudy);
                }
                else if (imgicon == "sunny") // in case sunny conditions exist at night in the api response
                {
                    imgCondition.SetImageResource(Resource.Drawable.sunny);
                }
                else if (imgicon == "hazy") // in case sunny conditions exist at night in the api response
                {
                    imgCondition.SetImageResource(Resource.Drawable.haze);
                }
            }
        }
    }
}