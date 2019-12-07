using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace CSharpMongoDBExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public class SanPham
        {
            public object _id { get; set; }
            public Int64 maSP { get; set; }
            public string tenSP { get; set; }
            public Int64 SL { get; set; }
            public string loaiSP { get; set; }
            public string mausac { get; set; }
            public double gia { get; set; }
        }
        private void btnGetProduct_Click(object sender, RoutedEventArgs e)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = client.GetDatabase("Quanlybanhang");

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("SanPham");
            List<BsonDocument> documents = collection.Find(new BsonDocument()).ToList();
            List<SanPham> dsProduct = new List<SanPham>();
            foreach (BsonDocument document in documents)
            {
                //Convert BsonDocument về C# model class:
                dsProduct.Add(BsonSerializer.Deserialize<SanPham>(document));
            }
            //đổ dữ liệu lên ListView
            lsvProduct.ItemsSource = dsProduct;
        }
        private int ComboBoxItemIndex;
        private void cmbSearch_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbFilter = sender as ComboBox;
            ComboBoxItemIndex = cbFilter.SelectedIndex;
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = client.GetDatabase("Quanlybanhang");

            IMongoCollection<BsonDocument> collection =
                database.GetCollection<BsonDocument>("SanPham");

            var builder = Builders<BsonDocument>.Filter;
            if (ComboBoxItemIndex == 0) { 
                var query = builder.Gt("gia", 5000000);
                List<BsonDocument> documents = collection.Find(query).ToList();
            //là tên của 1 Listbox nào đó
            List<SanPham> dsProduct = new List<SanPham>();
            foreach (BsonDocument document in documents)
            {
                dsProduct.Add(BsonSerializer.Deserialize<SanPham>(document));
            }
            //đổ dữ liệu lên ListView
            lsvProduct.ItemsSource = dsProduct;
            }
            else if(ComboBoxItemIndex ==1)
            {
                var query = builder.Lte("gia", 5000000);
                List<BsonDocument> documents = collection.Find(query).ToList();
                //là tên của 1 Listbox nào đó
                List<SanPham> dsProduct = new List<SanPham>();
                foreach (BsonDocument document in documents)
                {
                    dsProduct.Add(BsonSerializer.Deserialize<SanPham>(document));
                }
                //đổ dữ liệu lên ListView
                lsvProduct.ItemsSource = dsProduct;
            }

        }
       
        private void btnTimkiem_Click(object sender, RoutedEventArgs e)
        {
            SanPham p = DataContext as SanPham;
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = client.GetDatabase("Quanlybanhang");
            IMongoCollection<BsonDocument> collection =
                database.GetCollection<BsonDocument>("SanPham");
            
            //var builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("maSP", 2);
            BsonDocument documents = collection.Find(filter).FirstOrDefault();
            if (documents != null)
            {
                List<BsonDocument> documentss = collection.Find(filter).ToList();
                //là tên của 1 Listbox nào đó
                List<SanPham> dsProduct = new List<SanPham>();
                foreach (BsonDocument document in documentss)
                {
                    dsProduct.Add(BsonSerializer.Deserialize<SanPham>(document));
                }
                //đổ dữ liệu lên ListView
                lsvProduct.ItemsSource = dsProduct;
            }
            else
            {
                MessageBox.Show("Ko thấy nhe!");
            }
        }
        private void LoadAllProduct()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = client.GetDatabase("Quanlybanhang");
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("SanPham");
            List<BsonDocument> documents = collection.Find(new BsonDocument()).ToList();
            List<SanPham> dsProduct = new List<SanPham>();
            foreach (BsonDocument document in documents)
            {
                SanPham p = BsonSerializer.Deserialize<SanPham>(document);
                dsProduct.Add(p);
            }
            lsvProduct.ItemsSource = dsProduct;
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = client.GetDatabase("Quanlybanhang");
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("SanPham");
            BsonDocument document = new BsonDocument
            {
                { "tenSP",txbName.Text},
                { "SL",txbSL.Text},
                { "gia",txbPrice.Text},
                { "loaiSP",txbType.Text},
                { "maSP",txbMaSP.Text},
            };
            collection.InsertOne(document);
            MessageBox.Show("Đã lưu thành công");
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txbName.Clear();
            txbSL.Clear();
            txbPrice.Clear();
            txbType.Clear();
            txbType.Clear();
            txbMaSP.Clear();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //lấy Context là đối tượng SanPham được gán trong màn hình MainWindow
                SanPham p = DataContext as SanPham;

                MongoClient client = new MongoClient("mongodb://localhost:27017");
                IMongoDatabase database = client.GetDatabase("Quanlybanhang");
                IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("SanPham");
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("maSP", p.maSP);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                                                            .Set("tenSP", p.tenSP)
                                                            .Set("gia", p.gia)
                                                            .Set("SL", p.SL)
                                                            .Set("loaiSP", p.loaiSP);

                collection.UpdateOne(filter, update);
                MessageBox.Show("Update thành công");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

