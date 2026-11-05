using tounsiYoyo.Model;
namespace tounsiYoyo
{
    public partial class Backoffice : Form
    {
        public Backoffice()
        {
            InitializeComponent();
        }

        private void Export_button_click(object sender, EventArgs e)
        {
            Business business = new Business("COM-1510", "Tounsi Yoyo", "logo.png", new Special("PROD-1510", "Yoyo", 19.99f));
            business.Export();

            MessageBox.Show("Business exported to tounsi.json", "Exportation réussie", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
