using System.Text.Json;
using System.Windows.Forms;
using tounsiYoyo.Model;
namespace tounsiYoyo
{
    public partial class Backoffice : Form
    {
        public Backoffice()
        {
            InitializeComponent();
            string path = @"../../../../COM-1510.json";
            string jsonString = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<Business>(jsonString);
            BusinessName.Text = data.Name;
            BusinessID.Text = $"ID : {data.ID}";
            BusinessLogo.Image = Base64ToImage(data.Logo);

            SpecialName.Text = data.special.Name;
            SpecialID.Text = $"ID : {data.special.ID}";
            SpecialPrice.Value = (decimal)data.special.Price;
            SpecialLogo.Image = Base64ToImage(data.special.Picture);
        }

        private Image Base64ToImage(string base64String)
        {
            // On retire les métadonnées (ex: "data:image/jpeg;base64,") s'il y en a
            if (base64String.Contains(','))
            {
                base64String = base64String.Substring(base64String.IndexOf(',') + 1);
            }

            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes))
            {
                return new Bitmap(ms);
            }
        }

        public void Export_button_click(object sender, EventArgs e)
        {
            Business business = new Business("COM-1510", "Tounsi Yoyo", new Special("PROD-1510", "Yoyo", 19.9f));
            business.Export();

            MessageBox.Show("Business exported to COM-1510.json", "Exportation réussie", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
