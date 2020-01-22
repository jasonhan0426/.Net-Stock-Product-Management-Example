using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;


namespace Stock_Management_Software
{
    public partial class Stock : Form
    {
        DataTable dt = new DataTable();

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret= "P7VPrGmdOFbwnFJ4ZlLJLXZqePfl3BLvk2RkviAL",
            BasePath= "https://stock-inventory-list.firebaseio.com/"
        };

        IFirebaseClient client;


        public Stock()
        {
            InitializeComponent();
        }

        private void Stock_Load(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);

            if (client != null)
            {
                MessageBox.Show("Connection towards Firebase...");
            }

            comboBox1.SelectedIndex = 0;
            dataGridView1.Rows.Clear();

            dt.Columns.Add("Id");
            dt.Columns.Add("Name");
            dt.Columns.Add("Number");
            dt.Columns.Add("Status");

            dataGridView1.DataSource = dt;

            export();

            // load data
            //FirebaseResponse res = client.Get("Information/")

            /*DataTable jsondata = json_to_datatable();
            foreach (DataRow item in jsondata.Rows)
            {
                int n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = item["Id"].ToString();
                dataGridView1.Rows[n].Cells[1].Value = item["Name"].ToString();
                dataGridView1.Rows[n].Cells[1].Value = item["Number"].ToString();
                if ((bool)item["Status"])
                {
                    dataGridView1.Rows[n].Cells[2].Value = "Active";
                }
                else
                {
                    dataGridView1.Rows[n].Cells[2].Value = "Deactive";
                }
            }*/


        }

        private async void export()
        {
            dt.Rows.Clear();
            int i = 0;
            FirebaseResponse res1 = await client.GetTaskAsync("Counter/node");
            Counter obj1 = res1.ResultAs<Counter>();
            int counter = Convert.ToInt32(obj1.Id);

            while(true)
            {
                i++;
                try
                {
                    FirebaseResponse res2 = await client.GetTaskAsync("Information/"+i);
                    Data obj2 = res2.ResultAs<Data>();

                    DataRow row = dt.NewRow();
                    row["Id"] = obj2.Id;
                    row["Name"] = obj2.Name;
                    row["Number"] = obj2.Number;
                    if (obj2.status == 0)
                        row["Status"] = "Activate";
                    else
                        row["Status"] = "Deacivate";
                    dt.Rows.Add(row);

                }
                catch
                {

                }
            }
        }

        public DataTable json_to_datatable()
        {
            string URL = "https://stock-inventory-list.firebaseio.com/.json";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.ContentType = @"/json: charset-utf-8";
            HttpWebResponse res = req.GetResponse() as HttpWebResponse;
            using (Stream responsestream = res.GetResponseStream())
            {
                StreamReader read = new StreamReader(responsestream, Encoding.UTF8);
                string json = read.ReadToEnd();
                var table = JsonConvert.DeserializeObject<DataTable>(json);
                Console.WriteLine(table);
                return JsonConvert.DeserializeObject<DataTable>(json);
            }
        }

        // add data to firebase
        private async void button1_Click(object sender, EventArgs e)
        {
            FirebaseResponse res = await client.GetTaskAsync("Counter/node");
            Counter get = res.ResultAs<Counter>();

            var data = new Data
            {
                Id = (Convert.ToInt32(get.Id)+1).ToString(),
                Name = textBox2.Text,
                Number = textBox3.Text,
                status = comboBox1.SelectedIndex
            };

            SetResponse response = await client.SetTaskAsync("Information/"+data.Id, data);
            Data result = response.ResultAs<Data>();

            MessageBox.Show("Data Inserted " + result.Name);

            var counter = new Counter
            {
                Id = data.Id
            };
            SetResponse response1 = await client.SetTaskAsync("Counter/node", counter);
            export();
        }

        // delete data from firebase
        private async void button2_Click(object sender, EventArgs e)
        {
            var set = client.Delete("Information/" + textBox1.Text);

            MessageBox.Show("Data ID" + textBox1.Text + " Deleted");
            export();
        }

        // Delete all
        private async void button3_Click(object sender, EventArgs e)
        {
            FirebaseResponse res = await client.DeleteTaskAsync("Information");

            MessageBox.Show("All Data Deleted");
            export();
        }

        // Updata data
        private async void button4_Click(object sender, EventArgs e)
        {
            var data = new Data {
                Id = textBox1.Text,
                Name = textBox2.Text,
                Number = textBox3.Text,
                status = comboBox1.SelectedIndex
            };

            FirebaseResponse res = await client.UpdateTaskAsync("Information/"+textBox1.Text, data);
            Data result = res.ResultAs<Data>();
            MessageBox.Show("Data Update at Id: " + result.Id);
            export();
        }

        // Retrive Data
        private async void button5_Click(object sender, EventArgs e)
        {
            FirebaseResponse res = await client.GetTaskAsync("Information/" + textBox1.Text);
            Data result = res.ResultAs<Data>();

            textBox1.Text = result.Id;
            textBox2.Text = result.Name;
            textBox3.Text = result.Number;
            if (result.status == 0)
                comboBox1.SelectedIndex = 0;
            else
                comboBox1.SelectedIndex = 1;
        }
        export();
    }
}
