using KafeTekno.DATA;
using KafeTekno.UI.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeTekno.UI
{
    public partial class AnaForm : Form
    {
        KafeVeri db;
        public AnaForm()
        {
            InitializeComponent();
            VerileriOku();
            //OrnekUrunleriYukle();
            MasalariOlustur();
        }


        private void MasalariOlustur()
        {
            lvwMasalar.LargeImageList = BuyukImaJListesi();
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                ListViewItem lvi = new ListViewItem("Masa " + i);
                lvi.ImageKey = db.AktifSiparisler.Any(x => x.MasaNo == i) ? "dolu" : "bos";
                lvi.Tag = i;
                lvwMasalar.Items.Add(lvi);
            }
        }

        private ImageList BuyukImaJListesi()
        {
            ImageList il = new ImageList();
            il.ImageSize = new Size(64, 64);
            il.Images.Add("bos", Resources.bos);
            il.Images.Add("dolu", Resources.dolu);
            return il;
        }

        private void OrnekUrunleriYukle()
        {
            db.Urunler.Add(new Urun() { UrunAd = "Kola", BirimFiyat = 7.00m });
            db.Urunler.Add(new Urun() { UrunAd = "Ayran", BirimFiyat = 5.00m });
        }

        private void lvwMasalar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = lvwMasalar.SelectedItems[0];
            lvi.ImageKey = "dolu";
            int masaNo = (int)lvi.Tag;
            //MessageBox.Show(masaNo.ToString());
            Siparis siparis = SiparisBulYadaOlustur(masaNo);
            SiparisForm sf = new SiparisForm(db, siparis);
            sf.MasaTasindi += Sf_MasaTasindi;
            sf.ShowDialog();
            if (siparis.Durum != SiparisDurum.Aktif)
                lvi.ImageKey = "bos";
        }

        private void Sf_MasaTasindi(object sender, MasaTasindiEventArgs e)
        {
            MasaTasi(e.EskiMasaNo, e.YeniMasaNo);
        }

        private void MasaTasi(int eskiMasaNo, int yeniMasaNo)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                if ((int)lvi.Tag == eskiMasaNo)
                {
                    lvi.ImageKey = "bos";
                    lvi.Selected = false;
                }
                if ((int)lvi.Tag == yeniMasaNo)
                {
                    lvi.ImageKey = "dolu";
                    lvi.Selected = true;
                }
            }
        }

        private Siparis SiparisBulYadaOlustur(int masaNo)
        {
            Siparis siparis = db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo);// burada bulamazsa null döndürür.

            if (siparis == null)
            {
                siparis = new Siparis() { MasaNo = masaNo };
                db.AktifSiparisler.Add(siparis);
            }
            return siparis;
        }

        private void tsmiGecmis_Click(object sender, EventArgs e)
        {
            new GecmisSiparislerForm(db).ShowDialog();
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            new UrunlerForm(db).ShowDialog();
        }
        private void VerileriOku()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(json);
            }
            catch (Exception)
            {
                db = new KafeVeri();
                OrnekUrunleriYukle();
            }
        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            VerileriKaydet();
        }

        private void VerileriKaydet()
        {
            string json = JsonConvert.SerializeObject(db);
            File.WriteAllText("veri.json", json);
        }
    }
}
