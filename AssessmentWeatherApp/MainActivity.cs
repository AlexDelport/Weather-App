using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace AssessmentWeatherApp
{
    [Activity(Label = "Weather", MainLauncher = true, Icon = "@drawable/chancerain", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        // layout variables
        TextView txtCityName;
        ImageView imgTodayCondition;
        TextView txtTodayTemp;
        TextView txtTodayCondition;
        TextView txtTodayDate;
        TextView txtTodayMaxMin;
        Button btnTodayMoreInfo;

        TextView txtDay1Date;
        TextView txtDay2Date;
        TextView txtDay3Date;
        TextView txtDay4Date;
        TextView txtDay5Date;

        TextView txtMaxDay1;
        TextView txtMaxDay2;
        TextView txtMaxDay3;
        TextView txtMaxDay4;
        TextView txtMaxDay5;

        TextView txtMinDay1;
        TextView txtMinDay2;
        TextView txtMinDay3;
        TextView txtMinDay4;
        TextView txtMinDay5;

        ImageView imgDay1;
        ImageView imgDay2;
        ImageView imgDay3;
        ImageView imgDay4;
        ImageView imgDay5;

        // programming variables
        string apiCityLocation;
        string imgIcon;
        string passImgIcon;
        int currentHour;
        string nightTime = "";
        string currentDateTime;
        string todayWindDirection = "Wind Direction";
        string todayWindSpeed = "Wind Speed";
        string todayWindGustSpeed = "Wind Gust Speed";
        string todayFeelsLike = "Feels Like";
        string todayHumidity = "Humidity";
        string todayObservationTime = "Observation Time";

        RESTHandler objRest;
        RootObject objRootList;        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // determining hour of day, to display either day/night or dusk/dawn background
            currentHour = DateTime.Now.Hour;
            
            // used this site to determine dusk and dawn --http://rasnz.org.nz/in-the-sky/sun-rise-and-set

            // 8am up to and including 5pm
            if (currentHour >= 8 && currentHour < 18)
            {
                SetContentView(Resource.Layout.mainDay);
            }
            // 9pm up to and including 11pm or 12pm up to and including 4am
            else if (currentHour >= 20 && currentHour > 0 || currentHour >= 0 && currentHour < 5)
            {
                nightTime = "night";
                SetContentView(Resource.Layout.mainNight);
            }
            // 5am up to and including 7am or 6pm up to and including 8pm
            else if (currentHour >= 5 && currentHour < 8 || currentHour >= 18 && currentHour < 20)
            {
                SetContentView(Resource.Layout.mainDuskDawn);
            }

            // attach events to layout resources
            txtCityName = FindViewById<TextView>(Resource.Id.txtCityName);
            imgTodayCondition = FindViewById<ImageView>(Resource.Id.imgTodayCondition);
            txtTodayTemp = FindViewById<TextView>(Resource.Id.txtTodayTemp);
            txtTodayCondition = FindViewById<TextView>(Resource.Id.txtTodayCondition);
            txtTodayDate = FindViewById<TextView>(Resource.Id.txtTodayDate);
            txtTodayMaxMin = FindViewById<TextView>(Resource.Id.txtTodayMaxMin);
            btnTodayMoreInfo = FindViewById<Button>(Resource.Id.btnTodayMoreInfo);

            txtDay1Date = FindViewById<TextView>(Resource.Id.txtDay1Date);
            txtDay2Date = FindViewById<TextView>(Resource.Id.txtDay2Date);
            txtDay3Date = FindViewById<TextView>(Resource.Id.txtDay3Date);
            txtDay4Date = FindViewById<TextView>(Resource.Id.txtDay4Date);
            txtDay5Date = FindViewById<TextView>(Resource.Id.txtDay5Date);

            txtMaxDay1 = FindViewById<TextView>(Resource.Id.txtMaxDay1);
            txtMaxDay2 = FindViewById<TextView>(Resource.Id.txtMaxDay2);
            txtMaxDay3 = FindViewById<TextView>(Resource.Id.txtMaxDay3);
            txtMaxDay4 = FindViewById<TextView>(Resource.Id.txtMaxDay4);
            txtMaxDay5 = FindViewById<TextView>(Resource.Id.txtMaxDay5);

            txtMinDay1 = FindViewById<TextView>(Resource.Id.txtMinDay1);
            txtMinDay2 = FindViewById<TextView>(Resource.Id.txtMinDay2);
            txtMinDay3 = FindViewById<TextView>(Resource.Id.txtMinDay3);
            txtMinDay4 = FindViewById<TextView>(Resource.Id.txtMinDay4);
            txtMinDay5 = FindViewById<TextView>(Resource.Id.txtMinDay5);

            imgDay1 = FindViewById<ImageView>(Resource.Id.imgDay1);
            imgDay2 = FindViewById<ImageView>(Resource.Id.imgDay2);
            imgDay3 = FindViewById<ImageView>(Resource.Id.imgDay3);
            imgDay4 = FindViewById<ImageView>(Resource.Id.imgDay4);
            imgDay5 = FindViewById<ImageView>(Resource.Id.imgDay5);                      

            // setting a default city to display at start of app
            txtCityName.Text = "Hamilton";
            apiCityLocation = "zmw:00000.3.93186";

            // setting image to display while waiting for the call response
            imgTodayCondition.SetImageResource(Resource.Drawable.loading);

            LoadWeatherDataAsync();           
            
            btnTodayMoreInfo.Click += OnBtnTodayMoreInfo_Click;
        }

        public override bool OnCreateOptionsMenu(IMenu menu) // creating the menu and setting available options
        {
            menu.Add("Auckland");
            menu.Add("Christchurch");
            menu.Add("Hamilton");
            menu.Add("Wellington");
            menu.Add("Credits");
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item) 
        {
            var itemTitle = item.TitleFormatted.ToString();

            if (itemTitle != "Credits")
            {
                txtCityName.Text = itemTitle; // assinging city name depending on option chosen
            }

            if (itemTitle == "Auckland")
            {
                apiCityLocation = "NZ/Auckland";
            }
            else if (itemTitle == "Christchurch")
            {
                apiCityLocation = "zmw:00000.1.93780";
            }
            else if (itemTitle == "Hamilton")
            {
                apiCityLocation = "zmw: 00000.3.93186";
            }
            else if (itemTitle == "Wellington")
            {
                apiCityLocation = "zmw:00000.1.93436";
            }
            else if (itemTitle == "Credits")
            {
                var credits = new Intent(this, typeof(Credits));
                StartActivity(credits);
            }
                     
            LoadWeatherDataAsync(); 
            return base.OnOptionsItemSelected(item);
        }

        private async void LoadWeatherDataAsync() // using the API to get a response        
        {
            // displaying current date and time, obtained here to ensure always current with option selection
            currentDateTime = DateTime.Now.ToString();
            txtTodayDate.Text = currentDateTime;

            try
            {
                objRest = new RESTHandler(@"http://api.wunderground.com/api/0495186545d6aae5/geolookup/conditions/forecast10day/q/" + apiCityLocation + ".json");

                // as per Weather Underground, this is not the preferred method (NZ/CityName), rather get the location using NewZealand/CityName and then use the "l" from response.

                //objRest = new RESTHandler(@"http://api.wunderground.com/api/0495186545d6aae5/geolookup/conditions/forecast10day/q/NZ/" + txtCityName.Text + ".json");                

                objRootList = await objRest.ExecuteRequestAsync();

                imgIcon = "";
                DisplayTodaysWeather();
                DisplayForecastWeather();
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "Error: " + e.Message, ToastLength.Long);
            }
        }

        private void OnBtnTodayMoreInfo_Click(object sender, EventArgs e)
        {
            var moreInfo = new Intent(this, typeof(TodayMoreInfo));

            moreInfo.PutExtra("CityName", txtCityName.Text);
            moreInfo.PutExtra("Image", passImgIcon);
            moreInfo.PutExtra("Temp", txtTodayTemp.Text);
            moreInfo.PutExtra("Condition", txtTodayCondition.Text);
            moreInfo.PutExtra("MaxMinTemp", txtTodayMaxMin.Text);
            moreInfo.PutExtra("Date", txtTodayDate.Text);
            moreInfo.PutExtra("WindDirection", todayWindDirection);
            moreInfo.PutExtra("WindSpeed", todayWindSpeed);
            moreInfo.PutExtra("WindGustSpeed", todayWindGustSpeed);
            moreInfo.PutExtra("FeelsLike", todayFeelsLike);
            moreInfo.PutExtra("Humidity", todayHumidity);
            moreInfo.PutExtra("ObservationTime", todayObservationTime);

            StartActivity(moreInfo);
        }

        private void DisplayTodaysWeather() // displaying today's/current data received from the response
        {
            imgIcon = objRootList.current_observation.icon;
            passImgIcon = imgIcon;

            if (nightTime != "night")
            {
                GetWeatherImageTodayDay(imgIcon); // day images, includes dusk and dawn
            }
            else
            {
                GetWeatherImageTodayNight(imgIcon); // night images
            }

            txtTodayTemp.Text = objRootList.current_observation.temp_c.ToString() + " °C";

            if (objRootList.current_observation.weather != "")
            {
                txtTodayCondition.Text = objRootList.current_observation.weather;
            }
            else
            {
                txtTodayCondition.Text = objRootList.forecast.simpleforecast.forecastday[0].conditions;
            }
            
            txtTodayMaxMin.Text = "Max  " + objRootList.forecast.simpleforecast.forecastday[0].high.celsius.
                ToString() + " °C / " + objRootList.forecast.simpleforecast.forecastday[0].low.celsius.ToString() + " °C  Min";

            todayWindDirection = objRootList.current_observation.wind_dir;
            todayWindSpeed = objRootList.current_observation.wind_kph.ToString();
            todayWindGustSpeed = objRootList.current_observation.wind_gust_kph;
            todayFeelsLike = objRootList.current_observation.feelslike_c;
            todayHumidity = objRootList.current_observation.relative_humidity;
            todayObservationTime = objRootList.current_observation.observation_time;
        }

        private void DisplayForecastWeather() // displaying the forecasted data received from the response
        {
            txtDay1Date.Text = objRootList.forecast.simpleforecast.forecastday[1].date.day.ToString() + " " + objRootList.forecast.simpleforecast.forecastday[1].date.monthname_short.ToString();
            txtMaxDay1.Text = objRootList.forecast.simpleforecast.forecastday[1].high.celsius.ToString();
            txtMinDay1.Text = objRootList.forecast.simpleforecast.forecastday[1].low.celsius.ToString();
            imgIcon = objRootList.forecast.simpleforecast.forecastday[1].icon;
            GetWeatherImageForecastDay1(imgIcon);

            txtDay2Date.Text = objRootList.forecast.simpleforecast.forecastday[2].date.day.ToString() + " " + objRootList.forecast.simpleforecast.forecastday[2].date.monthname_short.ToString();
            txtMaxDay2.Text = objRootList.forecast.simpleforecast.forecastday[2].high.celsius.ToString();
            txtMinDay2.Text = objRootList.forecast.simpleforecast.forecastday[2].low.celsius.ToString();
            imgIcon = objRootList.forecast.simpleforecast.forecastday[2].icon;
            GetWeatherImageForecastDay2(imgIcon);

            txtDay3Date.Text = objRootList.forecast.simpleforecast.forecastday[3].date.day.ToString() + " " + objRootList.forecast.simpleforecast.forecastday[3].date.monthname_short.ToString();
            txtMaxDay3.Text = objRootList.forecast.simpleforecast.forecastday[3].high.celsius.ToString();
            txtMinDay3.Text = objRootList.forecast.simpleforecast.forecastday[3].low.celsius.ToString();
            imgIcon = objRootList.forecast.simpleforecast.forecastday[3].icon;
            GetWeatherImageForecastDay3(imgIcon);

            txtDay4Date.Text = objRootList.forecast.simpleforecast.forecastday[4].date.day.ToString() + " " + objRootList.forecast.simpleforecast.forecastday[4].date.monthname_short.ToString();
            txtMaxDay4.Text = objRootList.forecast.simpleforecast.forecastday[4].high.celsius.ToString();
            txtMinDay4.Text = objRootList.forecast.simpleforecast.forecastday[4].low.celsius.ToString();
            imgIcon = objRootList.forecast.simpleforecast.forecastday[4].icon;
            GetWeatherImageForecastDay4(imgIcon);

            txtDay5Date.Text = objRootList.forecast.simpleforecast.forecastday[5].date.day.ToString() + " " + objRootList.forecast.simpleforecast.forecastday[5].date.monthname_short.ToString();
            txtMaxDay5.Text = objRootList.forecast.simpleforecast.forecastday[5].high.celsius.ToString();
            txtMinDay5.Text = objRootList.forecast.simpleforecast.forecastday[5].low.celsius.ToString();
            imgIcon = objRootList.forecast.simpleforecast.forecastday[5].icon;
            GetWeatherImageForecastDay5(imgIcon);
        }

        public void GetWeatherImageTodayDay(string imgicon) // determining and setting day image for today's weather
        {                                                   // dusk and dawn use day images too
            if (imgicon == "chanceflurries" || imgicon == "flurries")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.flurries);                
            }
            else if (imgicon == "chancerain")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.chancerain);
            }
            else if (imgicon == "chancesleet" || imgicon == "sleet")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.sleet);
            }
            else if (imgicon == "chancesnow")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.chancesnow);
            }
            else if (imgicon == "chancetstorms")
            {
               imgTodayCondition.SetImageResource(Resource.Drawable.chancetstorms);
            }
            else if (imgicon == "clear" || imgicon == "sunny")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.sunny);
            }
            else if (imgicon == "cloudy")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.cloudy);
            }
            else if (imgicon == "fog")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.fog);
            }
            else if (imgicon == "hazy")
            {
               imgTodayCondition.SetImageResource(Resource.Drawable.haze);
            }
            else if (imgicon == "mostlycloudy" || imgicon == "partlysunny")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.mostcloudy);
            }
            else if (imgicon == "partlycloudy" || imgicon == "mostlysunny")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.partcloudy);
            }
            else if (imgicon == "rain")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.rain);
            }
            else if (imgicon == "snow")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.snow);
            }
            else if (imgicon == "tstorms")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.tstorms);
            }
            else if (imgicon == "unknown" || imgicon == "")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.unknown);
            }
        }

        public void GetWeatherImageTodayNight(string imgicon) // determining and setting night image for today's weather
        {
            if (imgicon == "chanceflurries" || imgicon == "chancesnow" || imgicon == "flurries" || imgicon == "snow")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.nsnow);
            }
            else if (imgicon == "chancerain" || imgicon == "rain")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.nrain);
            }
            else if (imgicon == "chancesleet" || imgicon == "sleet")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.sleet);
            }            
            else if (imgicon == "chancetstorms" || imgicon == "tstorms")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.ntstorms);
            }
            else if (imgicon == "clear")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.nclear);
            }
            else if (imgicon == "cloudy")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.cloudy);
            }
            else if (imgicon == "fog")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.fog);
            }
            else if (imgicon == "hazy")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.haze);
            }
            else if (imgicon == "mostlycloudy")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.nmostcloudy);
            }
            else if (imgicon == "partlycloudy")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.npartcloudy);
            }
            else if (imgicon == "unknown" || imgicon == "")
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.unknown);
            }            
            else if (imgicon == "mostlysunny") // in case sunny conditions exist at night in the api response
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.partcloudy);
            }
            else if (imgicon == "partlysunny") // in case sunny conditions exist at night in the api response
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.mostcloudy);
            }
            else if (imgicon == "sunny") // in case sunny conditions exist at night in the api response
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.sunny);
            }
            else if (imgicon == "hazy") // in case sunny conditions exist at night in the api response
            {
                imgTodayCondition.SetImageResource(Resource.Drawable.haze);
            }
        }

        public void GetWeatherImageForecastDay1(string imgicon) // determining and setting image for Day 1 forecasted weather
        {
            if (imgicon == "chanceflurries" || imgicon == "flurries")
            {
                imgDay1.SetImageResource(Resource.Drawable.flurries);
            }
            else if (imgicon == "chancerain")
            {
                imgDay1.SetImageResource(Resource.Drawable.chancerain);
            }
            else if (imgicon == "chancesleet" || imgicon == "sleet")
            {
                imgDay1.SetImageResource(Resource.Drawable.sleet);
            }
            else if (imgicon == "chancesnow")
            {
                imgDay1.SetImageResource(Resource.Drawable.chancesnow);
            }
            else if (imgicon == "chancetstorms")
            {
                imgDay1.SetImageResource(Resource.Drawable.chancetstorms);
            }
            else if (imgicon == "clear" || imgicon == "sunny")
            {
                imgDay1.SetImageResource(Resource.Drawable.sunny);
            }
            else if (imgicon == "cloudy")
            {
                imgDay1.SetImageResource(Resource.Drawable.cloudy);
            }
            else if (imgicon == "fog")
            {
                imgDay1.SetImageResource(Resource.Drawable.fog);
            }
            else if (imgicon == "hazy")
            {
                imgDay1.SetImageResource(Resource.Drawable.haze);
            }
            else if (imgicon == "mostlycloudy" || imgicon == "partlysunny")
            {
                imgDay1.SetImageResource(Resource.Drawable.mostcloudy);
            }
            else if (imgicon == "partlycloudy" || imgicon == "mostlysunny")
            {
                imgDay1.SetImageResource(Resource.Drawable.partcloudy);
            }
            else if (imgicon == "rain")
            {
                imgDay1.SetImageResource(Resource.Drawable.rain);
            }
            else if (imgicon == "snow")
            {
                imgDay1.SetImageResource(Resource.Drawable.snow);
            }
            else if (imgicon == "tstorms")
            {
                imgDay1.SetImageResource(Resource.Drawable.tstorms);
            }
            else if (imgicon == "unknown" || imgicon == "")
            {
                imgDay1.SetImageResource(Resource.Drawable.unknown);
            }
        }
        public void GetWeatherImageForecastDay2(string imgicon) // determining and setting image for Day 2 forecasted weather
        {
            if (imgicon == "chanceflurries" || imgicon == "flurries")
            {
                imgDay2.SetImageResource(Resource.Drawable.flurries);
            }
            else if (imgicon == "chancerain")
            {
                imgDay2.SetImageResource(Resource.Drawable.chancerain);
            }
            else if (imgicon == "chancesleet" || imgicon == "sleet")
            {
                imgDay2.SetImageResource(Resource.Drawable.sleet);
            }
            else if (imgicon == "chancesnow")
            {
                imgDay2.SetImageResource(Resource.Drawable.chancesnow);
            }
            else if (imgicon == "chancetstorms")
            {
                imgDay2.SetImageResource(Resource.Drawable.chancetstorms);
            }
            else if (imgicon == "clear" || imgicon == "sunny")
            {
                imgDay2.SetImageResource(Resource.Drawable.sunny);
            }
            else if (imgicon == "cloudy")
            {
                imgDay2.SetImageResource(Resource.Drawable.cloudy);
            }
            else if (imgicon == "fog")
            {
                imgDay2.SetImageResource(Resource.Drawable.fog);
            }
            else if (imgicon == "hazy")
            {
                imgDay2.SetImageResource(Resource.Drawable.haze);
            }
            else if (imgicon == "mostlycloudy" || imgicon == "partlysunny")
            {
                imgDay2.SetImageResource(Resource.Drawable.mostcloudy);
            }
            else if (imgicon == "partlycloudy" || imgicon == "mostlysunny")
            {
                imgDay2.SetImageResource(Resource.Drawable.partcloudy);
            }
            else if (imgicon == "rain")
            {
                imgDay2.SetImageResource(Resource.Drawable.rain);
            }
            else if (imgicon == "snow")
            {
                imgDay2.SetImageResource(Resource.Drawable.snow);
            }
            else if (imgicon == "tstorms")
            {
                imgDay2.SetImageResource(Resource.Drawable.tstorms);
            }
            else if (imgicon == "unknown" || imgicon == "")
            {
                imgDay2.SetImageResource(Resource.Drawable.unknown);
            }
        }
        public void GetWeatherImageForecastDay3(string imgicon) // determining and setting image for Day 3 forecasted weather
        {
            if (imgicon == "chanceflurries" || imgicon == "flurries")
            {
                imgDay3.SetImageResource(Resource.Drawable.flurries);
            }
            else if (imgicon == "chancerain")
            {
                imgDay3.SetImageResource(Resource.Drawable.chancerain);
            }
            else if (imgicon == "chancesleet" || imgicon == "sleet")
            {
                imgDay3.SetImageResource(Resource.Drawable.sleet);
            }
            else if (imgicon == "chancesnow")
            {
                imgDay3.SetImageResource(Resource.Drawable.chancesnow);
            }
            else if (imgicon == "chancetstorms")
            {
                imgDay3.SetImageResource(Resource.Drawable.chancetstorms);
            }
            else if (imgicon == "clear" || imgicon == "sunny")
            {
                imgDay3.SetImageResource(Resource.Drawable.sunny);
            }
            else if (imgicon == "cloudy")
            {
                imgDay3.SetImageResource(Resource.Drawable.cloudy);
            }
            else if (imgicon == "fog")
            {
                imgDay3.SetImageResource(Resource.Drawable.fog);
            }
            else if (imgicon == "hazy")
            {
                imgDay3.SetImageResource(Resource.Drawable.haze);
            }
            else if (imgicon == "mostlycloudy" || imgicon == "partlysunny")
            {
                imgDay3.SetImageResource(Resource.Drawable.mostcloudy);
            }
            else if (imgicon == "partlycloudy" || imgicon == "mostlysunny")
            {
                imgDay3.SetImageResource(Resource.Drawable.partcloudy);
            }
            else if (imgicon == "rain")
            {
                imgDay3.SetImageResource(Resource.Drawable.rain);
            }
            else if (imgicon == "snow")
            {
                imgDay3.SetImageResource(Resource.Drawable.snow);
            }
            else if (imgicon == "tstorms")
            {
                imgDay3.SetImageResource(Resource.Drawable.tstorms);
            }
            else if (imgicon == "unknown" || imgicon == "")
            {
                imgDay3.SetImageResource(Resource.Drawable.unknown);
            }
        }
        public void GetWeatherImageForecastDay4(string imgicon) // determining and setting image for Day 4 forecasted weather
        {
            if (imgicon == "chanceflurries" || imgicon == "flurries")
            {
                imgDay4.SetImageResource(Resource.Drawable.flurries);
            }
            else if (imgicon == "chancerain")
            {
                imgDay4.SetImageResource(Resource.Drawable.chancerain);
            }
            else if (imgicon == "chancesleet" || imgicon == "sleet")
            {
                imgDay4.SetImageResource(Resource.Drawable.sleet);
            }
            else if (imgicon == "chancesnow")
            {
                imgDay4.SetImageResource(Resource.Drawable.chancesnow);
            }
            else if (imgicon == "chancetstorms")
            {
                imgDay4.SetImageResource(Resource.Drawable.chancetstorms);
            }
            else if (imgicon == "clear" || imgicon == "sunny")
            {
                imgDay4.SetImageResource(Resource.Drawable.sunny);
            }
            else if (imgicon == "cloudy")
            {
                imgDay4.SetImageResource(Resource.Drawable.cloudy);
            }
            else if (imgicon == "fog")
            {
                imgDay4.SetImageResource(Resource.Drawable.fog);
            }
            else if (imgicon == "hazy")
            {
                imgDay4.SetImageResource(Resource.Drawable.haze);
            }
            else if (imgicon == "mostlycloudy" || imgicon == "partlysunny")
            {
                imgDay4.SetImageResource(Resource.Drawable.mostcloudy);
            }
            else if (imgicon == "partlycloudy" || imgicon == "mostlysunny")
            {
                imgDay4.SetImageResource(Resource.Drawable.partcloudy);
            }
            else if (imgicon == "rain")
            {
                imgDay4.SetImageResource(Resource.Drawable.rain);
            }
            else if (imgicon == "snow")
            {
                imgDay4.SetImageResource(Resource.Drawable.snow);
            }
            else if (imgicon == "tstorms")
            {
                imgDay4.SetImageResource(Resource.Drawable.tstorms);
            }
            else if (imgicon == "unknown" || imgicon == "")
            {
                imgDay4.SetImageResource(Resource.Drawable.unknown);
            }
        }
        public void GetWeatherImageForecastDay5(string imgicon) // determining and setting image for Day 5 forecasted weather
        {
            if (imgicon == "chanceflurries" || imgicon == "flurries")
            {
                imgDay5.SetImageResource(Resource.Drawable.flurries);
            }
            else if (imgicon == "chancerain")
            {
                imgDay5.SetImageResource(Resource.Drawable.chancerain);
            }
            else if (imgicon == "chancesleet" || imgicon == "sleet")
            {
                imgDay5.SetImageResource(Resource.Drawable.sleet);
            }
            else if (imgicon == "chancesnow")
            {
                imgDay5.SetImageResource(Resource.Drawable.chancesnow);
            }
            else if (imgicon == "chancetstorms")
            {
                imgDay5.SetImageResource(Resource.Drawable.chancetstorms);
            }
            else if (imgicon == "clear" || imgicon == "sunny")
            {
                imgDay5.SetImageResource(Resource.Drawable.sunny);
            }
            else if (imgicon == "cloudy")
            {
                imgDay5.SetImageResource(Resource.Drawable.cloudy);
            }
            else if (imgicon == "fog")
            {
                imgDay5.SetImageResource(Resource.Drawable.fog);
            }
            else if (imgicon == "hazy")
            {
                imgDay5.SetImageResource(Resource.Drawable.haze);
            }
            else if (imgicon == "mostlycloudy" || imgicon == "partlysunny")
            {
                imgDay5.SetImageResource(Resource.Drawable.mostcloudy);
            }
            else if (imgicon == "partlycloudy" || imgicon == "mostlysunny")
            {
                imgDay5.SetImageResource(Resource.Drawable.partcloudy);
            }
            else if (imgicon == "rain")
            {
                imgDay5.SetImageResource(Resource.Drawable.rain);
            }
            else if (imgicon == "snow")
            {
                imgDay5.SetImageResource(Resource.Drawable.snow);
            }
            else if (imgicon == "tstorms")
            {
                imgDay5.SetImageResource(Resource.Drawable.tstorms);
            }
            else if (imgicon == "unknown" || imgicon == "")
            {
                imgDay5.SetImageResource(Resource.Drawable.unknown);
            }
        }
    }
}

