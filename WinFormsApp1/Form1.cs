using System.Text.RegularExpressions;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        List<string> cities = new List<string>();
        List<string> coords = new List<string>();
        HttpClient client = new HttpClient();
        Regex rgx = new Regex("(\"country\":\"(?<Country>[A-Z]*)\")|(\"name\":\"(?<Name>[A-Za-z]*)\")|(\"temp\":(?<Temp>[0-9.]*))|(\"description\":\"(?<Desc>[A-Za-z\\s]*)\")");

        struct Weather
        {
            public string Country { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Temp { get; set; }
        }
        public Form1()
        {
            InitializeComponent();
            button1.Click += button_click;

            using (StreamReader reader = new StreamReader("city.txt"))
            {
                string line;
                string[] fields;
                
                while(!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    fields = line.Split('\t');

                    cities.Add(fields[0]);
                    coords.Add(fields[1]);
                }
            }

            listBox1.Items.AddRange(cities.ToArray());
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
         
        }

        async Task<Weather> GetWeather(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();

            MatchCollection matches = rgx.Matches(result);

            return new Weather()
            {
                Country = matches[2].Groups["Country"].Value,
                Name = "",
                Description = matches[0].Groups["Desc"].Value,
                Temp = matches[1].Groups["Temp"].Value
            };
        }
        private async void button_click(object? sender, EventArgs e)
        {
            
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Choise country");
                return;
            }

            string[] coordXY = coords[listBox1.SelectedIndex].Split(',');
            Weather weather = await GetWeather($"https://api.openweathermap.org/data/2.5/weather?lat={coordXY[0]}&lon={coordXY[1].Trim()}&appid=b3aa469ecf9912946416bcda21666d49");

            MessageBox.Show(weather.Temp);
        }
    }
}