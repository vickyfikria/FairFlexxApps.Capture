using FairFlexxApps.Capture.ViewModels;
using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FairFlexxApps.Capture.ViewModels.NewLeadFlows;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FairFlexxApps.Capture.Views.ViewCells.TopMenuViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchContentView : ContentView
    {
        public SketchContentView()
        {
            InitializeComponent();
            //clearButton.HeightRequest = 50 / App.DisplayScaleFactor;
            //clearButton.WidthRequest = 100 / App.DisplayScaleFactor;
        }
        private bool checkLoad;
        private async void LoadSketch()
        {
            var vm = (NewLeadTemplatePageViewModel)BindingContext;
            vm.MyEvent += async (IEnumerable<Point> point) => await GetByteImage(point);
            if (!checkLoad)
            {
                var pointTemp = await vm?.LoadPoint();
                if (pointTemp != null)
                {
                    signatureView.Points = pointTemp;
                }
                checkLoad = true;
            }
        }

        private void Stack_SizeChanged(object sender, System.EventArgs e)
        {
            LoadSketch();
        }

        public async Task<byte[]> GetByteImage(IEnumerable<Point> point)
        {
            signatureView.Points = point;
            if (!signatureView.IsBlank)
            {
                try
                {
                    Stream img = await signatureView.GetImageStreamAsync(SignatureImageFormat.Jpeg, strokeColor: Color.Black, fillColor: Color.White);
                    //BinaryReader br = new BinaryReader(img);
                    //br.BaseStream.Position = 0;
                    //byte[] image = br.ReadBytes((int)img.Length); 
                    var signatureMemoryStream = (MemoryStream)img;
                    byte[] data = signatureMemoryStream.ToArray();
                    return data;
                    //display the signature
                    //ImageSource imageSource = null;
                    //imageSource = ImageSource.FromStream(() => new MemoryStream(image));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            return null;
        }
    }
}