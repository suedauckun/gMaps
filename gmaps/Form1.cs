using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.ComponentModel.DataAnnotations;


namespace gmaps
{
    public partial class Form1 : Form
    {
        public delegate void ricdegis(string text);
        public Form1()
        {
            InitializeComponent();
        }

        private void map_Load(object sender, EventArgs e)
        {
            List<double> lat = new List<double>();
            List<double> lng = new List<double>();
            string dosya_yolu = @"C:\Users\suckun\Desktop\latlng4.plt";
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string oku = sw.ReadLine();
            string[] parcala;
            int i = 0;
            while (oku != null)
            {
                parcala = oku.Split(',');
                lat.Add(Convert.ToDouble(parcala[0]));
                lng.Add(Convert.ToDouble(parcala[1]));
                oku = sw.ReadLine();
                i++;
            }
            sw.Close();
            fs.Close();

            map.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            //rotada arrayleri kullan
            List<PointLatLng> noktalar = new List<PointLatLng>();
            for (int j = 0; j < lat.Count; j++)
            {
                GMapOverlay markersOverlay = new GMapOverlay("markers");
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat[j], lng[j]), GMarkerGoogleType.green);
                markersOverlay.Markers.Add(marker);
                map.Overlays.Add(markersOverlay);
                noktalar.Add(new PointLatLng(lat[j], lng[j]));
                map.Position = new GMap.NET.PointLatLng(lat[j], lng[j]);
            }

            GMapOverlay rota = new GMapOverlay("rota");
            GMapRoute rotaciz = new GMapRoute(noktalar, "rota");
            rotaciz.Stroke = new Pen(Color.Red, 3);
            rota.Routes.Add(rotaciz);
            map.Overlays.Add(rota);
            map.MinZoom = 5; // Minimum Zoom Level
            map.MaxZoom = 100; // Maximum Zoom Level
            map.Zoom = 15; // Current Zoom Level

        }

        private void button6_Click(object sender, EventArgs e)////ham koordinat yolla
        {
            try
            {
                Socket istemciBaglantisi = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                istemciBaglantisi.Connect(IPAddress.Parse("192.168.43.199"), 4000);
                string dosyaAdi = "krdnt.plt";// "Your File Name"; 
                string dosyaYolu = @"C:\Users\suckun\Desktop\";//Your File Path;
                byte[] dosyaAdiByte = Encoding.ASCII.GetBytes(dosyaAdi);
                byte[] fileData = File.ReadAllBytes(dosyaYolu + dosyaAdi);
                byte[] istemciData = new byte[4 + dosyaAdiByte.Length + fileData.Length];
                byte[] fileNameLen = BitConverter.GetBytes(dosyaAdiByte.Length);
                fileNameLen.CopyTo(istemciData, 0);
                dosyaAdiByte.CopyTo(istemciData, 4);
                fileData.CopyTo(istemciData, 4 + dosyaAdiByte.Length);
                istemciBaglantisi.Send(istemciData);
                MessageBox.Show("Dosya gönderildi.");
                istemciBaglantisi.Close();
            }
            catch
            {
                MessageBox.Show("Dosya gönderilemedi");
            }
        }

        private void button4_Click(object sender, EventArgs e)/////// indirgenmis sorgu al ve ciz
        {
            try
            {
                TcpListener dinleyiciSoket = new TcpListener(System.Net.IPAddress.Any, 4000);
                dinleyiciSoket.Start();
                //Bağlanan bir istemciyi kabul et ve bağlantıyı oluştur
                Socket istemciSoketi = dinleyiciSoket.AcceptSocket();
                byte[] istemciData1 = new byte[1024 * 5000];
                string alinanYol = "C:/Users/suckun/Desktop/";
                int alinanYolBytesLen = istemciSoketi.Receive(istemciData1);
                int fileNameLen1 = BitConverter.ToInt32(istemciData1, 0);
                string dosyaAdi1 = Encoding.ASCII.GetString(istemciData1, 4, fileNameLen1);
                BinaryWriter bYaz = new BinaryWriter(File.Open(alinanYol + dosyaAdi1, FileMode.Append));
                bYaz.Write(istemciData1, 4 + fileNameLen1, alinanYolBytesLen - 4 - fileNameLen1);
                istemciSoketi.Close();
                bYaz.Close();
                dinleyiciSoket.Stop();
                MessageBox.Show("Dosya alındı");
            }

            catch
            {
                MessageBox.Show("File Receiving fail.");
            }
          
        }

        private void imap_MouseClick(object sender, MouseEventArgs e)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            GMapOverlay polyOverlay = new GMapOverlay("poligonlar");
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                lat = (imap.FromLocalToLatLng(e.X, e.Y).Lat);
                lng = (imap.FromLocalToLatLng(e.X, e.Y).Lng);
                textBox3.Text = lat.ToString();
                textBox4.Text = lng.ToString();
            }
            double a = lat;
            double b = lng;
            if (textBox1.Text == "" && textBox2.Text == "")
            {
                MessageBox.Show("Textboxlar boş bırakılamaz");
            }
            else
            {
                double c = Convert.ToDouble(textBox2.Text);///boy
                double d = Convert.ToDouble(textBox1.Text);///en
                points.Add(new PointLatLng(a, b));////dikdortgen ciz
                points.Add(new PointLatLng(a + c, b));
                points.Add(new PointLatLng(a + c, b + d));
                points.Add(new PointLatLng(a, b + d));
                GMapPolygon poligon = new GMapPolygon(points, "poligon");
                poligon.Fill = new SolidBrush(Color.FromArgb(0, Color.Blue));
                poligon.Stroke = new Pen(Color.Blue, 2);
                polyOverlay.Polygons.Add(poligon);
                imap.Overlays.Add(polyOverlay);
            }
            string dosya_yolu2 = @"C:\Users\suckun\Desktop\krdnt2.plt";
            FileStream fs2 = new FileStream(dosya_yolu2, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw2 = new StreamWriter(fs2);
            sw2.WriteLine(a + " " + b + " " + textBox2.Text + " " + textBox1.Text);
            sw2.Flush();

        }

        private void button8_Click(object sender, EventArgs e)////indirgenmis koordinat yolla
        {
            try
            {
                Socket istemciBaglantisi1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                istemciBaglantisi1.Connect(IPAddress.Parse("192.168.43.199"), 4000);
                string dosyaAdi2 = "krdnt2.plt";// "Your File Name"; 
                string dosyaYolu1 = @"C:\Users\suckun\Desktop\";//Your File Path;
                byte[] dosyaAdiByte = Encoding.ASCII.GetBytes(dosyaAdi2);
                byte[] fileData = File.ReadAllBytes(dosyaYolu1 + dosyaAdi2);
                byte[] istemciData2 = new byte[4 + dosyaAdiByte.Length + fileData.Length];
                byte[] fileNameLen2 = BitConverter.GetBytes(dosyaAdiByte.Length);
                fileNameLen2.CopyTo(istemciData2, 0);
                dosyaAdiByte.CopyTo(istemciData2, 4);
                fileData.CopyTo(istemciData2, 4 + dosyaAdiByte.Length);
                istemciBaglantisi1.Send(istemciData2);
                MessageBox.Show("Dosya gönderildi.");
                istemciBaglantisi1.Close();
            }
            catch
            {
                MessageBox.Show("Dosya gönderilemedi");
            }
        }

        private void imap_Load(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)/////ham sorgudan doneni göster
        {
            List<double> lat = new List<double>();
            List<double> lng = new List<double>();
            string dosya_yolu = @"C:\Users\suckun\Desktop\hms.txt";
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string oku = sw.ReadLine();
            string[] parcala;
            int i = 0;
            while (oku != null)////dosyadan oku listlere at
            {
                parcala = oku.Split(',');
                lat.Add(Convert.ToDouble(parcala[0]));
                lng.Add(Convert.ToDouble(parcala[1]));
                oku = sw.ReadLine();
                i++;
            }
            sw.Close();
            fs.Close();

            map.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            //rotada arrayleri kullan
            List<PointLatLng> point = new List<PointLatLng>();
            for (int j = 0; j < lat.Count; j++)/////////dosyadan okudugu verileri haritada gösterir
            {
                GMapOverlay markersOverlay = new GMapOverlay("isaretleyici");
                GMarkerGoogle isaretleyici = new GMarkerGoogle(new PointLatLng(lat[j], lng[j]), GMarkerGoogleType.blue);
                markersOverlay.Markers.Add(isaretleyici);
                map.Overlays.Add(markersOverlay);
                point.Add(new PointLatLng(lat[j], lng[j]));
                map.Position = new GMap.NET.PointLatLng(lat[j], lng[j]);
            }
        }

        private void button5_Click(object sender, EventArgs e)////// ham sorgu al
        {
            try
            {
                TcpListener dinleyiciSoket1 = new TcpListener(System.Net.IPAddress.Any, 4000);
                dinleyiciSoket1.Start();
                Socket istemciSoketi1 = dinleyiciSoket1.AcceptSocket();
                byte[] istemciData3 = new byte[1024 * 5000];
                string alinanYol2 = "C:/Users/suckun/Desktop/";
                int alinanYol2BytesLen = istemciSoketi1.Receive(istemciData3);
                int fileNameLen3 = BitConverter.ToInt32(istemciData3, 0);
                string dosyaAdi2 = Encoding.ASCII.GetString(istemciData3, 4, fileNameLen3);
                BinaryWriter bYaz = new BinaryWriter(File.Open(alinanYol2 + dosyaAdi2, FileMode.Append));
                bYaz.Write(istemciData3, 4 + fileNameLen3, alinanYol2BytesLen - 4 - fileNameLen3);
                //soketleri kapat
                istemciSoketi1.Close();
                bYaz.Close();
                dinleyiciSoket1.Stop();
                MessageBox.Show("Dosya alındı");
            }
            catch
            {
                MessageBox.Show("File Receiving fail.");
            }

        }

        private void button1_Click(object sender, EventArgs e)////ham veri gonder
        {
            try
            {
                Socket istemciBaglantisi3 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                istemciBaglantisi3.Connect(IPAddress.Parse("192.168.43.199"), 4000);
                string dosyaAdi3 = "latlng4.plt";// "Your File Name"; 
                string dosyaYolu2 = @"C:\Users\suckun\Desktop\";//Your File Path;
                byte[] dosyaAdi3Byte = Encoding.ASCII.GetBytes(dosyaAdi3);
                byte[] fileData = File.ReadAllBytes(dosyaYolu2 + dosyaAdi3);
                byte[] istemciData4 = new byte[4 + dosyaAdi3Byte.Length + fileData.Length];
                byte[] fileNameLen4 = BitConverter.GetBytes(dosyaAdi3Byte.Length);
                fileNameLen4.CopyTo(istemciData4, 0);
                dosyaAdi3Byte.CopyTo(istemciData4, 4);
                fileData.CopyTo(istemciData4, 4 + dosyaAdi3Byte.Length);
                istemciBaglantisi3.Send(istemciData4);
                MessageBox.Show("Dosya gönderildi.");
                istemciBaglantisi3.Close();
            }
            catch
            {
                MessageBox.Show("Dosya gönderilemedi");
            }

        }

        private void button2_Click(object sender, EventArgs e)////indirgenmis veri al ekrana bas
        {
            try
            {
                TcpListener dinleyiciSoket2 = new TcpListener(System.Net.IPAddress.Any, 4000);
                dinleyiciSoket2.Start();
                Socket istemciSoketi2 = dinleyiciSoket2.AcceptSocket();
                byte[] istemciData5 = new byte[1024 * 5000];
                string alinanYol3 = "C:/Users/suckun/Desktop/";
                int alinanYol3BytesLen = istemciSoketi2.Receive(istemciData5);
                int fileNameLen5 = BitConverter.ToInt32(istemciData5, 0);
                string dosyaAdi3 = Encoding.ASCII.GetString(istemciData5, 4, fileNameLen5);
                BinaryWriter bYaz = new BinaryWriter(File.Open(alinanYol3 + dosyaAdi3, FileMode.Append));
                bYaz.Write(istemciData5, 4 + fileNameLen5, alinanYol3BytesLen - 4 - fileNameLen5);
                //soketleri kapat
                istemciSoketi2.Close();
                bYaz.Close();
                dinleyiciSoket2.Stop();
                MessageBox.Show("Dosya alındı");
            }
            catch
            {
                MessageBox.Show("File Receiving fail.");
            }

            List<double> lat = new List<double>();
            List<double> lng = new List<double>();
            string dosya_yolu = @"C:\Users\suckun\Desktop\ind.plt";
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string oku = sw.ReadLine();
            string[] parcala;
            int i = 0;
            while (oku != null)
            {
                parcala = oku.Split(',');
                lat.Add(Convert.ToDouble(parcala[0]));
                lng.Add(Convert.ToDouble(parcala[1]));
                oku = sw.ReadLine();
                i++;
            }
            sw.Close();
            fs.Close();

            imap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            //rotada arrayleri kullan
            List<PointLatLng> noktalar = new List<PointLatLng>();
            for (int j = 0; j < lat.Count; j++)
            {
                GMapOverlay markersOverlay = new GMapOverlay("markers");
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat[j], lng[j]), GMarkerGoogleType.green);
                markersOverlay.Markers.Add(marker);
                imap.Overlays.Add(markersOverlay);
                noktalar.Add(new PointLatLng(lat[j], lng[j]));
                imap.Position = new GMap.NET.PointLatLng(lat[j], lng[j]);
            }

            GMapOverlay rota = new GMapOverlay("rota");
            GMapRoute rotaciz = new GMapRoute(noktalar, "rota");
            rotaciz.Stroke = new Pen(Color.Red, 3);
            rota.Routes.Add(rotaciz);
            imap.Overlays.Add(rota);
            imap.MinZoom = 5; // Minimum Zoom Level
            imap.MaxZoom = 100; // Maximum Zoom Level
            imap.Zoom = 15; // Current Zoom Level

        }

        private void button3_Click(object sender, EventArgs e)//////// Oran alma
        {
            TcpListener dinleyiciSoket3 = new TcpListener(System.Net.IPAddress.Any, 4000);
            dinleyiciSoket3.Start();
            Socket istemciSoketi3 = dinleyiciSoket3.AcceptSocket();
            NetworkStream agAkisi = new NetworkStream(istemciSoketi3);
            BinaryReader binaryOkuyucu = new BinaryReader(agAkisi);
            double alinanMetin = binaryOkuyucu.ReadDouble();
            textBox5.Text = alinanMetin.ToString();///oran
            istemciSoketi3.Close();
            dinleyiciSoket3.Stop();
        }

        double lat, lng;

        private void button9_Click(object sender, EventArgs e)
        {
            List<double> lat = new List<double>();
            List<double> lng = new List<double>();
            string dosya_yolu = @"C:\Users\suckun\Desktop\indir.txt";
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string oku = sw.ReadLine();
            string[] parcala;
            int i = 0;
            while (oku != null)
            {
                parcala = oku.Split(',');
                lat.Add(Convert.ToDouble(parcala[0]));
                lng.Add(Convert.ToDouble(parcala[1]));
                oku = sw.ReadLine();
                i++;
            }
            sw.Close();
            fs.Close();

            imap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            //rotada arrayleri kullan
            List<PointLatLng> point = new List<PointLatLng>();
            for (int j = 0; j < lat.Count; j++)
            {
                GMapOverlay markersOverlay = new GMapOverlay("isaretleyici");
                GMarkerGoogle isaretleyici = new GMarkerGoogle(new PointLatLng(lat[j], lng[j]), GMarkerGoogleType.blue);
                markersOverlay.Markers.Add(isaretleyici);
                imap.Overlays.Add(markersOverlay);
                point.Add(new PointLatLng(lat[j], lng[j]));
                imap.Position = new GMap.NET.PointLatLng(lat[j], lng[j]);
            }
        }

        private void map_MouseClick(object sender, MouseEventArgs e)////ham veri uzerine mouse tiklaninca dortgen ciz
        {
            List<PointLatLng> points = new List<PointLatLng>();
            GMapOverlay polyOverlay = new GMapOverlay("poligonlar");
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                lat = (map.FromLocalToLatLng(e.X, e.Y).Lat);
                lng = (map.FromLocalToLatLng(e.X, e.Y).Lng);
                textBox3.Text = lat.ToString();///boy 
                textBox4.Text = lng.ToString();///en
            }
            double a = lat;
            double b = lng;
            if (textBox1.Text == "" && textBox2.Text == "")
            {
                MessageBox.Show("Textboxlar boş bırakılamaz");
            }
            else
            {
                double c = Convert.ToDouble(textBox2.Text);
                double d = Convert.ToDouble(textBox1.Text);
                points.Add(new PointLatLng(a, b));
                points.Add(new PointLatLng(a + c, b));
                points.Add(new PointLatLng(a + c, b + d));
                points.Add(new PointLatLng(a, b + d));
                GMapPolygon polygon = new GMapPolygon(points, "poligon");
                polygon.Fill = new SolidBrush(Color.FromArgb(0, Color.Blue));
                polygon.Stroke = new Pen(Color.Blue, 2);
                polyOverlay.Polygons.Add(polygon);
                map.Overlays.Add(polyOverlay);
            }
            string dosya_yolu2 = @"C:\Users\suckun\Desktop\krdnt.plt";////koordinatlari al bir dosyaya yaz
            FileStream fs2 = new FileStream(dosya_yolu2, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw2 = new StreamWriter(fs2);
            sw2.WriteLine(a + " " + b + " "+textBox2.Text+" "+textBox1.Text);
            sw2.Flush();
          
        }
    }
}