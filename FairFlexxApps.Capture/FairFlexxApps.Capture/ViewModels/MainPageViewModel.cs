using System;
using System.Threading.Tasks;
using FairFlexxApps.Capture.Enums;
using FairFlexxApps.Capture.Interfaces;
using FairFlexxApps.Capture.Interfaces.HttpService;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.Utilities;
using FairFlexxApps.Capture.ViewModels.Base;
using Prism.Navigation;
using Prism.Services;
using RestDemo.Model;
using SkiaSharp;

namespace FairFlexxApps.Capture.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService, IHttpRequest httpRequest, IPageDialogService dialogService, IFileService fileService)
            : base(navigationService: navigationService, httpRequest: httpRequest, dialogService: dialogService, fileService: fileService)
        {
            Title = "Main Page";
        }

        #region Properties

        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { SetProperty(ref _notes, value); }
        }

        private string _sketch;
        public string Sketch
        {
            get { return _sketch; }
            set { SetProperty(ref _sketch, value); }
        }

        private string _card;
        public string Card
        {
            get { return _card; }
            set { SetProperty(ref _card, value); }
        }

        private string _attachment;
        public string Attachment
        {
            get { return _attachment; }
            set { SetProperty(ref _attachment, value); }
        }

        private string _object;
        public string Object
        {
            get { return _object; }
            set { SetProperty(ref _object, value); }
        }

        #endregion

        #region GetXMLStructure

        public async Task<Menu> GetXMLStructure()
        {
            Menu result = null;

            await Task.Run(async () =>
            {
                var url = "http://api.androidhive.info/pizza/?format=xml";

                var response = await HttpRequest.GetObjectTaskAsync<Menu>(requestUri: url);

                result = await GetXmlStructureCallBack(response: response);

            });


            return result;
        }

        private async Task<Menu> GetXmlStructureCallBack(Menu response)
        {
            if (response == null)
            {
                await DeviceExtension.BeginInvokeOnMainThreadAsync(async () =>
                {
                    await DialogService.DisplayAlertAsync("Caution",
                        "Please on the Internet!",
                        null, "Close");
                });

                return null;
            }
            else
            {
                return response;
            }

        }

        #endregion

        #region Override Navigate back to

        public void OnNavigatedNewTo(NavigationParameters parameters)
        {
            if (parameters != null)
            {

            }

        }

        #endregion

        #region Save Form

        private SKBitmap _saveBitmap;
        public SKBitmap SaveBitmap
        {
            get => _saveBitmap;
            set => SetProperty(ref _saveBitmap, value);
        }
        public async Task SaveSketchSkiaSharp()
        {
            using (SKImage image = SKImage.FromBitmap(SaveBitmap))
            {
                SKData data = image.Encode();
                var resultSaveImgFile = await FileService.SaveTypeFile(eventName: "A", type: LeadType.Sketch,
                    byteImage: data.ToArray(), leadTypeModel: null);
            }
        }

        #endregion

        //private void UpdateScreen()
        //{
        //    var assembly = typeof(MainPage).GetTypeInfo().Assembly;
        //    Stream stream = assembly.GetManifestResourceStream("FairFlexxApps.Capture." + "tablet_event_template.xml");
        //    XDocument docs = XDocument.Load(stream);

        //    foreach (var item in docs.Descendants("menu"))
        //    {
        //        XmlPizzaDetails ObjPizzaItem = new XmlPizzaDetails();
        //        ObjPizzaItem.id = item.Element("id").Value.ToString();
        //        ObjPizzaItem.item = item.Element("item").Value.ToString();
        //        ObjPizzaLists.Add(ObjPizzaItem);
        //    }
        //}
    }
}