using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using FairFlexxApps.Capture.Constants;
using FairFlexxApps.Capture.Controls;
using FairFlexxApps.Capture.Enums;
using FairFlexxApps.Capture.Enums.Templates;
using FairFlexxApps.Capture.Interfaces;
using FairFlexxApps.Capture.Interfaces.LocalDatabase;
using FairFlexxApps.Capture.Localization;
using FairFlexxApps.Capture.Managers;
using FairFlexxApps.Capture.Models;
using FairFlexxApps.Capture.Models.LeadModels;
using FairFlexxApps.Capture.Models.Templates;
using FairFlexxApps.Capture.Models.Templates.Controls;
using FairFlexxApps.Capture.Models.Templates.Pages;
using FairFlexxApps.Capture.Services.HttpService;
using FairFlexxApps.Capture.Utilities;
using FairFlexxApps.Capture.ViewModels.Base;
using FairFlexxApps.Capture.Views.Commons.GeneratePdf;
using FairFlexxApps.Capture.Views.Commons.LayoutTemplates;
using FairFlexxApps.Capture.Views.NewLeadFlows;
using FairFlexxApps.Capture.Views.Popups;
using FairFlexxApps.Capture.Views.ViewCells;
using FairFlexxApps.Capture.Views.ViewCells.TopMenuViews;
using FotoScanSdk.Abstractions.Models;

using Prism.Commands;
using Prism.Navigation;
using Mopups;
using SkiaSharp;
using VCardReader;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Internals;
using Device = Microsoft.Maui.Controls.Device;
using NavigationParameters = Prism.Navigation.NavigationParameters;

namespace FairFlexxApps.Capture.ViewModels.NewLeadFlows
{
    public class NewLeadTemplatePageViewModel : ViewModelBase
    {
        #region NewLeadTemplatePageViewModel

        //public static NewLeadTemplatePageViewModel Instance { get; private set; }
        public NewLeadTemplatePageViewModel(INavigationService navigationService, ISqLiteService sqLiteService, IFileService fileService)
            : base(navigationService: navigationService, sqliteService: sqLiteService, fileService: fileService)
        {
            HeightOfContent = App.ScreenHeight - 125;
            ScannerResult = new ObservableCollection<ScannerResult>();

            DataTopMemu = new TopMenu();

            SaveCommand = new DelegateCommand(SaveExcute);
            SwitchCardButtonHandlerCommand = new DelegateCommand(async () => await SwitchCardButtonHandler());

            ScanCommand = new DelegateCommand<string>(ScanExe);
            BackImageCommand = new DelegateCommand(BackImage);
            NextImageCommand = new DelegateCommand(NextImage);
            PositionCommand = new DelegateCommand(PositionChange);
            DetailImageCommand = new DelegateCommand<LeadTypeModel>(DetailImageExe);

            DeleteCommand = new DelegateCommand(DeleteExe);
            SaveSketchCommand = new Command(SaveSketchExecute);
            ClearSketchCommand = new Command(ClearSketchExecute);

            LeadTypeFiles = new ObservableCollection<LeadTypeModel>();

            Instance = this;

            IsVisibleSideMenuCommand =
                new Command<string>(async (swipeDirection) => await IsVisibleSideMenuExecute(swipeDirection));

            HiddenTopMenuCommand = new DelegateCommand(HiddenTopMenuExcute);
        }
        #endregion

        #region Properties

        public static event EventHandler<EventArgs> ImageView;

        //private static NewLeadTemplatePageViewModel _instance;

        public static NewLeadTemplatePageViewModel Instance { get; private set; }

        private int _indexOfLanguage;
        public int IndexOfLanguage
        {
            get => _indexOfLanguage;
            set => SetProperty(ref _indexOfLanguage, value);
        }

        private bool _isShownTopMenu = true;
        public bool IsShownTopMenu
        {
            get => _isShownTopMenu;
            set => SetProperty(ref _isShownTopMenu, value);
        }

        private StatusOfLeadModel _statusLead;
        public StatusOfLeadModel StatusLead
        {
            get => _statusLead;
            set => SetProperty(ref _statusLead, value);
        }

        private int _position = 0;
        public int Position
        {
            get => _position;
            set
            {
                SetProperty(ref _position, value);
                UpdatePositionArrowIcon();
            }
        }

        private ObservableCollection<ScannerResult> _scannerResult;
        public ObservableCollection<ScannerResult> ScannerResult
        {
            get => _scannerResult;
            private set
            {
                SetProperty(ref _scannerResult, value);
                //RaisePropertyChanged("ScannerResult");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<SideMenu> _dataSideMenu;
        public ObservableCollection<SideMenu> DataSideMenu
        {
            get => _dataSideMenu;
            set => SetProperty(ref _dataSideMenu, value);
        }

        private ObservableCollection<View> _layoutPages = new ObservableCollection<View>();
        public ObservableCollection<View> LayoutPages
        {
            get => _layoutPages;
            set => SetProperty(ref _layoutPages, value);
        }

        private ObservableCollection<View> _layoutPagesTopMenu = new ObservableCollection<View>();
        public ObservableCollection<View> LayoutPagesTopMenu
        {
            get => _layoutPagesTopMenu;
            set => SetProperty(ref _layoutPagesTopMenu, value);
        }

        private TopMenu _topMenu;
        public TopMenu DataTopMemu
        {
            get => _topMenu;
            set => SetProperty(ref _topMenu, value);
        }

        private double _heightOfContent;
        public double HeightOfContent
        {
            get => _heightOfContent;
            set => SetProperty(ref _heightOfContent, value);
        }

        private SideMenu _selected;
        public SideMenu Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        private ObservableCollection<LeadTypeModel> _leadTypeFiles;
        public ObservableCollection<LeadTypeModel> LeadTypeFiles
        {
            get => _leadTypeFiles;
            set => SetProperty(ref _leadTypeFiles, value);
        }

        private LeadTypeModel _scannerResultSelected;
        public LeadTypeModel ScannerResultSelected
        {
            get => _scannerResultSelected;
            set => SetProperty(ref _scannerResultSelected, value);
        }

        private bool _notesSelected;
        public bool NotesSelected
        {
            get => _notesSelected;
            set => SetProperty(ref _notesSelected, value);
        }

        private bool _attachmentSelected;
        public bool AttachmentSelected
        {
            get => _attachmentSelected;
            set => SetProperty(ref _attachmentSelected, value);
        }

        private bool _sketchSelected;
        public bool SketchSelected
        {
            get => _sketchSelected;
            set => SetProperty(ref _sketchSelected, value);
        }

        private bool _objectSelected;
        public bool ObjectSelected
        {
            get => _objectSelected;
            set => SetProperty(ref _objectSelected, value);
        }

        private IEnumerable<Point> _currentSignature;
        public IEnumerable<Point> CurrentSignature
        {
            get => _currentSignature;
            set => SetProperty(ref _currentSignature, value);
        }

        private Point[] _savedSignature;
        public Point[] SavedSignature
        {
            get => _savedSignature;
            set => SetProperty(ref _savedSignature, value);
        }

        private EventModel _eventModel;
        public EventModel EventModel
        {
            get => _eventModel;
            set => SetProperty(ref _eventModel, value);
        }

        private SKBitmap _saveBitmap;
        public SKBitmap SaveBitmap
        {
            get => _saveBitmap;
            set => SetProperty(ref _saveBitmap, value);
        }

        private bool _drawOnSketch;
        public bool DrawOnSketch
        {
            get => _drawOnSketch;
            set => SetProperty(ref _drawOnSketch, value);
        }
        private Models.Templates.Page HeaderNotePage { get; set; }
        #endregion

        #region OnAppearing

        public override async void OnAppear()
        {

        }

        #endregion

        public static Template Template { get; set; }
        public static Template TemplateTemp { get; set; }

        #region Override Navigate new to

        public override async void OnNavigatedNewTo(INavigationParameters parameters)
        {
            if (parameters != null)
            {
                if (parameters.ContainsKey(ParamKey.IndexOfLanguage.ToString()))
                {
                    IndexOfLanguage = (int)parameters[ParamKey.IndexOfLanguage.ToString()];
                }

                if (parameters.ContainsKey(ParamKey.EventModel.ToString()))
                {
                    Title = ((EventModel)parameters[ParamKey.EventModel.ToString()]).Name;
                    EventModel = (EventModel)parameters[ParamKey.EventModel.ToString()];
                }

                if (parameters.ContainsKey(nameof(StatusOfLeadModel)) &&
                    parameters.ContainsKey(ParamKey.LeadModel.ToString()))
                {
                    StatusLead = ((StatusOfLeadModel)parameters[nameof(StatusOfLeadModel)]);
                    NewLead = (LeadModel)parameters[ParamKey.LeadModel.ToString()];
                }

                if (parameters.ContainsKey(ParamKey.LeadWithCard.ToString()))
                {
                    if ((bool)parameters[ParamKey.LeadWithCard.ToString()])
                    {
                        NewLead.IsLeadWithCard = true;
                    }
                }
                IndexOfLanguage = (StatusLead == StatusOfLeadModel.CreateLead ? IndexOfLanguage : NewLead.IndexOfLanguage);
                UpdateUi(indexOfLanguage: (StatusLead == StatusOfLeadModel.CreateLead ? IndexOfLanguage : NewLead.IndexOfLanguage));

                //if (NewLead.IsLeadWithCard)
                //{
                //    // No having Visitor page
                //    // Insert item to index = 1
                //    var item = new SideMenu();
                //    item.SideMenuID = "Visitor";
                //    item.IsMissed = true;
                //    item.ItemStatus = SideMenuItemStatus.None;
                //    //item.ImageSource = "ic_circle";
                //    DataSideMenu.Insert(index: 1, item: item);

                //    var layout = new LeadWithCardContentView(isMissedVisitorPage: false);
                //    LayoutPages.Insert(1, layout);
                //    Template.Pages.Insert(index: 1, item: new Models.Templates.Page());
                //}
                //else
                //{
                //    if (DataSideMenu.All(item => item.SideMenuID != "Visitor"))
                //    {
                //        // Insert item to index = 1
                //        var item = new SideMenu();
                //        item.SideMenuID = "Visitor";
                //        item.IsMissed = true;
                //        item.ItemStatus = SideMenuItemStatus.None;
                //        //item.ImageSource = "ic_circle";
                //        DataSideMenu.Insert(index: 1, item: item);

                //        var layout = new LeadWithCardContentView(isMissedVisitorPage: true);
                //        LayoutPages.Insert(1, layout);
                //        Template.Pages.Insert(index: 1, item: new Models.Templates.Page());
                //    }
                //}

                if (parameters.ContainsKey(ParamKey.EventModel.ToString()))
                {
                    Title = ((EventModel)parameters[ParamKey.EventModel.ToString()]).Name;
                    EventModel = (EventModel)parameters[ParamKey.EventModel.ToString()];

                }

                SetLayoutTopMenu();
            }

        }

        #endregion

        #region Override Navigate back to

        public override void OnNavigatedBackTo(INavigationParameters parameters)
        {
            if (parameters != null)
            {
                if (parameters.ContainsKey(ParamKey.NewLead.ToString()))
                {
                    NewLead = (LeadModel)parameters[ParamKey.NewLead.ToString()];

                    SqLiteService.Insert(NewLead);
                    SqLiteService.DeleteAll(NewLead.LeadTypesList);
                    SqLiteService.InsertAll(NewLead.LeadTypesList);

                    UpdateLeadTypeFiles();

                    ScannerResult = new ObservableCollection<ScannerResult>(ScannerResult);
                } else if (parameters.ContainsKey(ParamKey.VCardResult.ToString()))
                {
                    VCard = (VCard)parameters[ParamKey.VCardResult.ToString()];
                    var visitorPage = Template.Pages.FirstOrDefault(p => p.Short == "visitor");
                    var visitorBox = visitorPage.Boxs.FirstOrDefault(b => b.Id == "visitor");
                    var visitorBoxInputs = new List<Input>();
                    visitorBox.Body.Rows.ToList<Row>(); .ForEach(r => visitorBoxInputs.AddRange(r.Inputs));
                    foreach (var input in visitorBoxInputs)
                    {

                        switch (input.InputId)
                        {
                            case "mr":
                                input.Value = VCard.Gender == VCardReader.Gender.Male ? "true" : "false";
                                break;
                            case "ms":
                                input.Value = VCard.Gender == VCardReader.Gender.Female ? "true" : "false";
                                break;
                            case "email":
                                input.Value = VCard.EmailAddresses?.FirstOrDefault()?.Address;
                                break;
                            case "firstname":
                                input.Value = VCard.GivenName;
                                break;
                            case "lastname":
                                input.Value = VCard.FamilyName;
                                break;
                            case "position":
                                input.Value = VCard.Title;
                                break;
                            case "phone":
                                input.Value = VCard.Phones?.FirstOrDefault()?.FullNumber;
                                break;
                            case "mobile":
                                input.Value = (VCard.Phones?.Count() ?? 0) > 1 ? VCard.Phones.LastOrDefault()?.FullNumber : "";
                                break;
                            default:
                                break;
                        }
                    }

                    var companyBox = visitorPage.Boxs.FirstOrDefault(b => b.Id == "company");
                    var companyBoxInputs = new List<Input>();
                    companyBox.Body.Rows.ForEach(r => companyBoxInputs.AddRange(r.Inputs));
                    foreach (var input in companyBoxInputs)
                    {

                        switch (input.InputId)
                        {
                            case "company":
                                input.Value = VCard.Organization;
                                break;
                            case "addrline1":
                                input.Value = !String.IsNullOrEmpty(VCard.DeliveryAddresses?.FirstOrDefault()?.Region) ? $"{VCard.DeliveryAddresses?.FirstOrDefault()?.Street}, {VCard.DeliveryAddresses?.FirstOrDefault()?.Region}" : $"{VCard.DeliveryAddresses?.FirstOrDefault()?.Street}";
                                break;
                            case "addrline2":
                                input.Value = (VCard.DeliveryAddresses?.Count() ?? 0) > 1 ? !String.IsNullOrEmpty(VCard.DeliveryAddresses?.LastOrDefault()?.Region) ? $"{VCard.DeliveryAddresses?.LastOrDefault()?.Street}, {VCard.DeliveryAddresses?.LastOrDefault()?.Region}" : $"{VCard.DeliveryAddresses?.LastOrDefault()?.Street}": "";
                                break;
                            case "postcode":
                                input.Value = VCard.DeliveryAddresses?.FirstOrDefault()?.PostalCode;
                                break;
                            case "city":
                                input.Value = VCard.DeliveryAddresses?.FirstOrDefault()?.City;
                                break;
                            case "country":
                                input.Value = VCard.DeliveryAddresses?.FirstOrDefault()?.Country;
                                break;
                            default:
                                break;
                        } 
                    }

                    VisitorPage = PageTemplate.GetPageTemplate(languageIndex: _indexOfLanguage, pagesTemplate: visitorPage, totalPages: Template.Pages.Count, pageIndex: _preIndexPage);
                    LayoutPages[_preIndexPage] = VisitorPage;
                    ScanQrCodeHandler?.Invoke(VisitorPage, EventArgs.Empty);
                }

            }

        }

        #endregion

        #region UpdateUi

        private async void UpdateUi(int indexOfLanguage)
        {
            try
            {
                var innerXml = await ReadFileXml();

                //Template = GetTemplateModel(innerXml: innerXml);
                Template = GetTemplateModel(innerXml: !string.IsNullOrEmpty(NewLead.XmlForm) ? NewLead.XmlForm : EventModel.XmlFormat);

                //TemplateTemp = GetTemplateModel(innerXml: innerXml);
                TemplateTemp = GetTemplateModel(innerXml: !string.IsNullOrEmpty(NewLead.XmlForm) ? NewLead.XmlForm : EventModel.XmlFormat);

                //Debug.WriteLine("== Template Printed: ");
                //Debug.WriteLine(Template.ObjectToStringContent());

                var pageNeedToRemove = new ObservableCollection<Models.Templates.Page>(Template.Pages.Where(p => !p.Visible));
                HeaderNotePage = Template.Pages.FirstOrDefault(p => p.PageID == XMLConstants.HEADER_NOTE_PAGE_ID);
                
                if (HeaderNotePage != null)
                {
                    pageNeedToRemove.Add(HeaderNotePage);
                }
                foreach (var page in pageNeedToRemove)
                {
                    Template.Pages.Remove(page);
                }

                DataSideMenu = SideMenuTemplate.GetDataSideMenu(template: Template, languageIndex: indexOfLanguage);

                DataTopMemu = TopMenuTemplate.GetDataTopMenu(topMenuTemplate: Template.TopMenu);

                // Get Layout Pages
                LayoutPages = PageTemplate.GetAllPageTemplate(languageIndex: indexOfLanguage, template: Template);

                foreach (var item in Template.Pages)
                {
                    //if (item.SideMenuID.ToLower() == "visitor")
                    if (item.LeadWithCard != null && !item.LeadWithCard.Visible)
                    {
                        var index = Template.Pages.IndexOf(item);
                        VisitorPage = LayoutPages[index];
                        break;
                    }
                }

                if (NewLead.IsLeadWithCard)
                {
                    //// No having Visitor page
                    //// Insert item to index = 1
                    //var item = new SideMenu();
                    //item.SideMenuID = "Visitor";
                    //item.IsMissed = true;
                    //item.ItemStatus = SideMenuItemStatus.None;
                    ////item.ImageSource = "ic_circle";
                    //DataSideMenu.Insert(index: 1, item: item);

                    //var layout = new LeadWithCardContentView(isMissedVisitorPage: false);
                    //LayoutPages.Insert(1, layout);
                    //Template.Pages.Insert(index: 1, item: new Models.Templates.Page());
                    foreach (var item in Template.Pages)
                    {
                        //if (item.SideMenuID.ToLower() == "visitor")
                        if (item.LeadWithCard != null && !item.LeadWithCard.Visible)
                        {
                            var index = Template.Pages.IndexOf(item);
                            LayoutPages[index] = new LeadWithCardContentView(isMissedVisitorPage: false);
                            break;
                        }
                    }
                }

                if (StatusLead == StatusOfLeadModel.UpdateLead)
                {
                    LeadType = LeadType.BusinessCard;
                    UpdateLeadTypeFiles();
                    GetStatusSideMenu();
                }

                Selected = DataSideMenu[0];
                NewLeadTemplatePhonePage.Instance.SetDefaultSideMenu(Selected);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }


        }

        #endregion

        #region ReadFileXML

        private async Task<string> ReadFileXml()
        {
            //var v = (MainPageViewModel)BindingContext;

            //var test = await ViewModel?.GetXMLStructure();


            var assembly = typeof(NewLeadTemplatePageViewModel).GetTypeInfo().Assembly;

            Stream stream = assembly.GetManifestResourceStream("FairFlexxApps.Capture." + "tablet_template.xml");
            XmlDocument docs = new XmlDocument();
            docs.Load(stream);

            return docs.InnerXml;
        }

        #endregion

        #region Set up Layout for Top Menu

        private void SetLayoutTopMenu()
        {
            if (NewLead.SketchBytes != null)
            {
                Stream stream = new MemoryStream(NewLead.SketchBytes);
                SaveBitmap = SKBitmap.Decode(stream);
            }

            LayoutPagesTopMenu.Add(new NotesContentView());
            LayoutPagesTopMenu.Add(new SketchSkiaSharpContentView());

            LayoutPagesTopMenu.Add(new ObjectContentView());
            LayoutPagesTopMenu.Add(new AttachmentContentView());
        }

        #endregion

        #region Update Lead Type Files

        private void UpdateLeadTypeFiles()
        {
            var dataFromDb = SqLiteService.GetList<LeadTypeModel>(lead => !lead.Id.Equals(Guid.Empty) && lead.Type == LeadType && lead.LeadId == NewLead.LeadId);
            if (dataFromDb != null && dataFromDb.Any())
            {
                LeadTypeFiles = new ObservableCollection<LeadTypeModel>(dataFromDb);

                foreach (var leadTypeFile in LeadTypeFiles)
                {
                    var scannerImage = SqLiteService.GetList<ScannerResult>(lead =>
                        !lead.Id.Equals(Guid.Empty) && lead.LeadId.Equals(leadTypeFile.Id));
                    leadTypeFile.ScannerResult = new ObservableCollection<ScannerResult>(scannerImage);
                }

                ScannerResultSelected = LeadTypeFiles.FirstOrDefault();
                ScannerResult = new ObservableCollection<ScannerResult>(LeadTypeFiles[0].ScannerResult);
                Position = 0;
                GetSourceFromByte();
            }
            else
            {
                LeadTypeFiles = new ObservableCollection<LeadTypeModel>();
                ScannerResult = new ObservableCollection<ScannerResult>();
            }
        }

        private void GetSourceFromByte()
        {
            if (LeadType == LeadType.BusinessCard)
            {
                foreach (var leadType in LeadTypeFiles)
                {
                    foreach (var sr in leadType.ScannerResult)
                    {
                        var stream = new MemoryStream(sr.ByteImage);
                        sr.Source = ImageSource.FromStream(() => stream);
                    }

                    leadType.BusinessCardSource = leadType.ScannerResult[leadType.Position].ByteImage;
                }
            }
            else
            {
                foreach (var sr in ScannerResult)
                {
                    var stream = new MemoryStream(sr.ByteImage);
                    sr.Source = ImageSource.FromStream(() => stream);
                }
            }
        }

        #endregion

        #region GetTemplateModel

        private Template GetTemplateModel(string innerXml)
        {
            Serializer ser = new Serializer();
            var template = new Template();
            template = ser.Deserialize<Template>(innerXml);
            return template;
        }

        #endregion

        #region Update

        private int _preIndexPage = -1;

        public View Update(SideMenu item)
        {
            var index = DataSideMenu.IndexOf(item);

            if (_preIndexPage != -1)
                DataSideMenu[_preIndexPage].ItemStatus = CheckSideMenuItemStatus(preIndex: _preIndexPage);
            else if (StatusLead == StatusOfLeadModel.UpdateLead)
                DataSideMenu[0].ItemStatus = CheckSideMenuItemStatus(0);
            else DataSideMenu[0].ItemStatus = SideMenuItemStatus.None;

            foreach (var sideMenuItem in DataSideMenu)
                sideMenuItem.Selected = false;
           
            
            Selected = DataSideMenu[index];
            DataSideMenu[index].Selected = true;
            DataSideMenu = new ObservableCollection<SideMenu>(DataSideMenu);

            LeadType = LeadType.BusinessCard;

            UpdateLeadTypeFiles();

            _preIndexPage = index;

            ResetTopMenuItemSelected();

            return LayoutPages[index];

        }

        #endregion

        #region Reset top menu item selected

        private void ResetTopMenuItemSelected()
        {
            NotesSelected = false;
            SketchSelected = false;
            ObjectSelected = false;
            AttachmentSelected = false;
        }

        #endregion

        #region Zoom

        protected override async void ZoomImageExe(object obj)
        {
            await CheckBusy(async () =>
            {
                if (obj == null)
                {
#if DEBUG
                    Debug.WriteLine("NULL");
#endif
                    return;
                }

                await PopupNavigation.Instance.PushAsync(new ZoomImagePopUp(((ScannerResult)obj).ByteImage));
            });
        }

        public async void ZoomImageCard(byte[] obj)
        {
            await CheckBusy(async () =>
            {
                if (obj == null)
                {
#if DEBUG
                    Debug.WriteLine("NULL");
#endif
                    return;
                }

                await PopupNavigation.Instance.PushAsync(new ZoomImagePopUp(obj));
            });
        }

        public async Task ZoomImageWithPos()
        {
            await CheckBusy(async () =>
            {
                await PopupNavigation.Instance.PushAsync(new ZoomImagePopUp(ScannerResult[Position].ByteImage));
            });
        }

        #endregion

        #region ScanCommand

        private LeadType LeadType { get; set; }

        private LeadModel _newLead;
        public LeadModel NewLead
        {
            get => _newLead;
            set => SetProperty(ref _newLead, value);
        }

        private VCard _vCard;
        public VCard VCard
        {
            get => _vCard;
            set => SetProperty(ref _vCard, value);
        }

        public ICommand ScanCommand { get; set; }

        private async void ScanExe(string leadType)
        {
            await CheckBusy(async () =>
            {
                var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                    cameraStatus = results[Permission.Camera];
                }

                if (cameraStatus == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    await CameraAction();
                }
                else
                {
                    await MessagePopup.Instance.Show(TranslateExtension.Get("PermissionsDenied"));
                    CrossPermissions.Current.OpenAppSettings();
                }
            });
        }

        private async Task CameraAction()
        {
            var scannerConfiguration = new ScannerConfiguration();
            if (LeadType == LeadType.Attachment)
            {

                scannerConfiguration.AutoSnappingEnabled = App.Configs.Attach_Auto;
                scannerConfiguration.MultiPageEnabled = App.Configs.Attach_Multi;
                scannerConfiguration.FlashEnabled = App.Configs.Attach_Flash;
            }
            else if (LeadType == LeadType.Object)
            {
                scannerConfiguration.MultiPageEnabled = App.Configs.Object_Multi;
                scannerConfiguration.AutoSnappingEnabled = false;
                scannerConfiguration.FlashEnabled = App.Configs.Object_Flash;
            }
            else
            {
                scannerConfiguration.MultiPageEnabled = App.Configs.Visitor_Multi;
                scannerConfiguration.AutoSnappingEnabled = App.Configs.Visitor_Auto;
                scannerConfiguration.FlashEnabled = App.Configs.Visitor_Flash;
            }

            var currentTitleList = new TitleList
            {
                ColorTitle = "Color",
                GreyScaleTitle = "Grey Scale",
                BlackAndWhiteTitle = "Black And White",
                SaveTitle = "Save",
                MultiTitle = "Multi",
                SingleTitle = "Single",
                AutoTitle = "Auto",
                ManualTitle = "Manual",
                CancelTitle = "Cancel",
                PageTitle = "Page",
                PagesTitle = "Pages",
                FindingObjectTitle = "Finding Object",
                MoveCloserTitle = "Move Closer",
                HoldStillTitle = "Hold Still",
                NotificationTitle = "Notification",
                FiveImagesWarningTitle = "Cannot take more than 5 images",
                LoadingTitle = "Loading...",
            };

            var scannedPageResult = await FotoScanSdk.FotoScanSdk.Current.LaunchDocumentScannerAsync(scannerConfiguration: scannerConfiguration, titleList: currentTitleList);

            if (scannedPageResult.Any())
            {
                var param = new NavigationParameters
                    {
                        {ParamKey.ScannerResult.ToString(), scannedPageResult},
                        {ParamKey.NewLead.ToString(), NewLead},
                        {ParamKey.LeadType.ToString(), LeadType},
                        {ParamKey.EventModel.ToString(), EventModel}
                    };
                await Navigation.NavigateAsync(PageManager.ImageViewerPage, param);
            }
        }

        #endregion

        #region BackImageCommand

        private bool _isVisibleBack;
        public bool IsVisibleBack
        {
            get => _isVisibleBack;
            set => SetProperty(ref _isVisibleBack, value);
        }

        public ICommand BackImageCommand { get; set; }
        private async void BackImage()
        {
            await CheckBusy(async () =>
            {
                Position--;
            });
        }
        #endregion

        #region NextImageCommand

        private bool _isVisibleNext;
        public bool IsVisibleNext
        {
            get => _isVisibleNext;
            set => SetProperty(ref _isVisibleNext, value);
        }

        public ICommand NextImageCommand { get; set; }
        private async void NextImage()
        {
            await CheckBusy(async () =>
            {
                Position++;
            });
        }
        #endregion

        #region UpdatePositionArrowIcon

        private void UpdatePositionArrowIcon()
        {
            // Back Image
            IsVisibleBack = Position != 0;

            // Next Image
            IsVisibleNext = Position != ScannerResult.Count - 1 && ScannerResult.Count != 1;

            if (ScannerResult.Any())
            {
                ScannerResult = new ObservableCollection<ScannerResult>(ScannerResult);
                var stream = new MemoryStream(ScannerResult[Position].ByteImage);
                ScannerResult[Position].Source = ImageSource.FromStream(() => stream);

                InvokeEventToShowImageInContentView();
            }
        }

        #endregion

        #region PositionCommand

        public ICommand PositionCommand { get; private set; }
        private async void PositionChange()
        {
            await CheckBusy(async () =>
            {
            });
        }

        #endregion

        #region Detail Image

        public ICommand DetailImageCommand { get; set; }
        private async void DetailImageExe(LeadTypeModel leadTypeFile)
        {
            await CheckBusy(async () =>
            {
                ScannerResult = new ObservableCollection<ScannerResult>(leadTypeFile.ScannerResult);

                GetSourceFromByte();

                ScannerResultSelected = leadTypeFile;

                Position = 0;

                //var param = new NavigationParameters
                //{
                //    {ParamKey.NewLead.ToString(), NewLead},
                //    {ParamKey.LeadType.ToString(), LeadType},
                //    {ParamKey.FileModel.ToString(), leadTypeFile }
                //};

                //await Navigation.NavigateAsync(PageManager.DetailImagePage, param);
            });
        }
        #endregion

        #region TopMenuItemSelectedCommand

        public ICommand TopMenuItemSelectedCommand { get; set; }
        public async Task<View> TopMenuItemSelectedExecute(string isSelected)
        {
            var index = 0;
            DataSideMenu[_preIndexPage].ItemStatus = CheckSideMenuItemStatus(preIndex: _preIndexPage);
            await CheckBusy(async () =>
            {
                NotesSelected = false;
                SketchSelected = false;
                ObjectSelected = false;
                AttachmentSelected = false;

                switch (isSelected)
                {
                    case nameof(NotesSelected):
                        NotesSelected = true;
                        index = 0;
                        break;
                    case nameof(SketchSelected):
                        SketchSelected = true;
                        index = 1;
                        break;
                    case nameof(ObjectSelected):
                        ObjectSelected = true;
                        LeadType = LeadType.Object;
                        index = 2;
                        break;
                    case "Attach.Selected":
                        AttachmentSelected = true;
                        LeadType = LeadType.Attachment;
                        index = 3;
                        break;
                }
            });

            var menuItem = DataSideMenu.Where(item => item.Selected).FirstOrDefault();
            var indexSideMenu = DataSideMenu.IndexOf(menuItem);
            if (indexSideMenu >= 0)
            {
                DataSideMenu[index: indexSideMenu].Selected = false;
                DataSideMenu = new ObservableCollection<SideMenu>(DataSideMenu);
            }

            UpdateLeadTypeFiles();

            ScannerResult = new ObservableCollection<ScannerResult>(ScannerResult);

            return LayoutPagesTopMenu[index];
        }        

        #endregion

        #region Delete execute

        public ICommand DeleteCommand { get; set; }

        private async void DeleteExe()
        {
            await CheckBusy(async () =>
            {
                var leadFile = GetLeadTypeFileCurrent();

                await ConfirmPopup.Instance.Show(
                    TranslateExtension.Get("DeleteDocument"),
                    TranslateExtension.Get("DeleteDocumentContent"),
                    TranslateExtension.Get("Cancel"),
                    acceptButtonText: TranslateExtension.Get("Delete"),
                    acceptCommand: new Command(async () =>
                    {
                        await LoadingPopup.Instance.Show(TranslateExtension.Get("Deleting"));

                        FileService.DeleteFile($"{leadFile.FilePath}");
                        switch (LeadType)
                        {
                            //case LeadType.Form:
                            //    NewLead.IsFormCreated = false;
                            //    NewLead.LeadTypesList.Remove(LeadTypeFile);
                            //    break;

                            case LeadType.BusinessCard:
                                NewLead.BusinessCardCountCreated--;
                                //NewLead.LeadTypesList.Remove(leadFile);
                                //LeadTypeFiles.Remove(leadFile);
                                break;

                            case LeadType.Attachment:
                                NewLead.IsAttachmentCreated = false;
                                //NewLead.LeadTypesList.Remove(leadFile);
                                break;

                            case LeadType.Object:
                                NewLead.IsObjectCreated = false;
                                //NewLead.LeadTypesList.Remove(leadFile);
                                break;

                        }

                        DeleteLeadTypeFile(leadFile: leadFile);

                        await Task.Delay(700);
                        await LoadingPopup.Instance.Hide();

                        var param = new NavigationParameters
                        {
                            {ParamKey.NewLead.ToString(), NewLead},
                            {ParamKey.LeadType.ToString(), LeadType}
                        };
                        //await Navigation.NavigateAsync(ManagerPage.GoBack(ManagerPage.CreateNewLeadPage, 2), param);
                    }));

            });
        }

        #region GetLeadTypeFileCurrent

        private LeadTypeModel GetLeadTypeFileCurrent()
        {
            var index = 0;
            foreach (var leadTypeFile in LeadTypeFiles)
            {
                if (leadTypeFile.ScannerResult.SequenceEqual(ScannerResult))
                {
                    index = LeadTypeFiles.IndexOf(leadTypeFile);
                    break; ;
                }
            }
            return LeadTypeFiles[index];
        }

        #endregion

        #region DeleteLeadTypeFile

        private void DeleteLeadTypeFile(LeadTypeModel leadFile)
        {
            var leadRemove = NewLead.LeadTypesList.Where(lead => lead.Id.Equals(leadFile.Id)).FirstOrDefault();
            NewLead.LeadTypesList.Remove(leadRemove);
            //LeadTypeFiles.Remove(leadFile);
            SqLiteService.Delete(leadFile);
            SqLiteService.DeleteAll(leadFile.ScannerResult);

            UpdateLeadTypeFiles();
        }

        #endregion

        #endregion

        #region ClearSketchCommand

        public ICommand ClearSketchCommand { get; }
        private void ClearSketchExecute()
        {
            var sketch = new ObservableCollection<Point>();
            CurrentSignature = sketch as IEnumerable<Point>;
            NewLead.StringSketchPoints = string.Empty;
            SaveSketchExecute();
        }

        #endregion

        #region SaveCommand

        public ICommand SaveCommand { get; set; }
        private async void SaveExcute()
        {
            await CheckBusy(async () =>
            {
                await LoadingPopup.Instance.Show();

                GetStatusSideMenu();

                if (StatusIconOfLeadModel() == StatusOfLeadModel.QuestionMark)
                {
                    DataSideMenu = new ObservableCollection<SideMenu>(DataSideMenu);
                    Selected = DataSideMenu[0];
                    await MessagePopup.Instance.Show(TranslateExtension.Get("HaveNotEnteredMandatoryField"));
                    return;
                }

                NewLead.IconStatus = StatusIconOfLeadModel();

                #region Save Notes

                if (!string.IsNullOrEmpty(NewLead.Notes))
                {
                    // Save note file
                    await SaveNotesFile();
                }

                #endregion

                #region Save Sketch

                if (DrawOnSketch)
                {
                    // Save sketch
                    await SaveSketchSkiaSharp();
                }

                #endregion

                Serializer s = new Serializer();
                //foreach (var page in Template.Pages)
                //{
                //    if (!page.Boxs.Any())
                //        Template.Pages.Remove(page);
                //}

                // Todo: Task 05.14.2019: remove Missed page
                if (DataSideMenu.Any(item => item.IsMissed))
                    Template.Pages.RemoveAt(1);

                #region Save Form Template To Pdf

                await SaveFormTemplateToPdf();

                #endregion

                foreach (var invisiblePage in TemplateTemp.Pages)
                {
                    if (!invisiblePage.Visible)
                    {
                        var index = TemplateTemp.Pages.IndexOf(invisiblePage);
                        Template.Pages.Insert(index: index, item: invisiblePage);
                    }
                }

                if (NewLead.IsLeadWithCard)
                {
                    for (int index = 0; index < Template.Pages.Count; index++)
                    {
                        if (Template.Pages[index].LeadWithCard != null && !Template.Pages[index].LeadWithCard.Visible)
                        {
                            Template.Pages[index] = TemplateTemp.Pages[index];
                        }
                    }
                }
                if(HeaderNotePage != null)
                {
                    HeaderNotePage.Boxs[0].Body.Rows[0].Inputs[0].Value = NewLead.Notes;
                    Template.Pages.Add(HeaderNotePage);
                }

                NewLead.XmlForm = s.ObjectToSerialize<Template>(Template);

                NewLead.EventId = EventModel.Id;

                NewLead.Name = NewLead.IsLeadWithCard ? "Lead with card" : (string.IsNullOrEmpty(NewLead.CompanyName) ? "Empty" : NewLead.CompanyName);

                NewLead.TimeStamp = (StatusLead.ToString() == StatusOfLeadModel.CreateLead.ToString())
                    ? DateTime.Now.ToString("dd.MM.yyyy HH:mm")
                    : NewLead.TimeStamp;

                NewLead.TimeStampId = (StatusLead.ToString() == StatusOfLeadModel.CreateLead.ToString())
                    ? DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
                    : NewLead.TimeStampId;

                NewLead.LeadName = $"{NewLead.TimeStamp} - {NewLead.Name}";


                // Save xml file
                #region Save XML

                await SaveXmlFile();

                #endregion

                if (StatusLead.ToString() == StatusOfLeadModel.CreateLead.ToString())
                {
                    EventModel.TotalLeads++;
                    if (EventModel.LatestUpdatedTime != DateTime.Today)
                        EventModel.TodayLeads = 1;
                    else
                        EventModel.TodayLeads++;
                    EventModel.LatestUpdatedTime = DateTime.Today;
                    SqLiteService.Insert(EventModel);
                    NewLead.IndexOfLanguage = IndexOfLanguage;

                }
                SqLiteService.Insert(NewLead);

                var param = new NavigationParameters
                {
                    { ParamKey.GoBackAndReload.ToString(), true },
                };

                await Task.Delay(1500);

                await LoadingPopup.Instance.Hide();

                await Navigation.GoBackToRootAsync(parameters: param);

            });
        }

        #region Save Form Template To Pdf

        private ObservableCollection<SKBitmap> _formTemplateBitmaps = new ObservableCollection<SKBitmap>();
        public ObservableCollection<SKBitmap> FormTemplateBitmaps
        {
            get => _formTemplateBitmaps;
            set => SetProperty(ref _formTemplateBitmaps, value);
        }

        private async Task SaveFormTemplateToPdf()
        {
            await DrawPdf.SaveFormTemplateToPdf(formTemplateBitmaps: FormTemplateBitmaps, template: Template, newLead: NewLead, indexOfLanguage: IndexOfLanguage);

            await SavePdf();
        }

        #region SavePdf

        public async Task SavePdf()
        {
            var bytes = new ObservableCollection<byte[]>();
            foreach (var template in FormTemplateBitmaps)
            {
                var wA4Size = 597.6f;
                var hA4Size = 842.3f;
                var a4Size = new SKBitmap((int)wA4Size, (int)hA4Size);
                template.ScalePixels(a4Size, SKFilterQuality.High);
                using (SKImage image = SKImage.FromPixels(a4Size.PeekPixels()))
                {
                    SKData data = image.Encode();
                    bytes.Add(data.ToArray());
                }

            }

            var pdfFiles = (SqLiteService.GetList<LeadTypeModel>(note =>
                !note.Id.Equals(Guid.Empty) && note.LeadId == NewLead.LeadId && note.Type == LeadType.FormPdf)).FirstOrDefault();

            var resultSaveImgFile = await FileService.SaveTypeFile(eventName: EventModel.Name, type: LeadType.FormPdf,
                        leadTypeModel: pdfFiles, bytesPdf: bytes);

            if (resultSaveImgFile.IsSaved)
            {
                var leadType = new LeadTypeModel()
                {
                    Type = resultSaveImgFile.Type,
                    FileName = resultSaveImgFile.FileName,
                    FilePath = resultSaveImgFile.LocalPath,
                    LeadId = NewLead.LeadId,
                    CreateDate = resultSaveImgFile.CreateDate
                };
                NewLead.LeadTypesList.Add(leadType);
                SqLiteService.Delete(pdfFiles);
                SqLiteService.Insert(leadType);
            }
        }

        #endregion   
        
        #endregion

        #region Save Notes File

        private async Task SaveNotesFile()
        {
            var noteFiles = (SqLiteService.GetList<LeadTypeModel>(note =>
                !note.Id.Equals(Guid.Empty) && note.LeadId == NewLead.LeadId && note.Type == LeadType.Note)).FirstOrDefault();
            var resultSaveNoteFile = await FileService.SaveTypeFile(eventName: EventModel.Name,
                type: LeadType.Note, content: NewLead.Notes, leadTypeModel: noteFiles);

            if (resultSaveNoteFile.IsSaved)
            {
                var leadType = new LeadTypeModel()
                {
                    Type = resultSaveNoteFile.Type,
                    FileName = resultSaveNoteFile.FileName,
                    FilePath = resultSaveNoteFile.LocalPath,
                    LeadId = NewLead.LeadId,
                    CreateDate = resultSaveNoteFile.CreateDate
                };
                NewLead.LeadTypesList.Add(leadType);
                SqLiteService.Delete(noteFiles);
                SqLiteService.Insert(leadType);
            }
        }

        #endregion

        #region Save Sketch Skia Sharp

        private async Task SaveSketchSkiaSharp()
        {
            using (SKImage image = SKImage.FromBitmap(SaveBitmap))
            {
                SKData data = image.Encode();
                var imageFiles = (SqLiteService.GetList<LeadTypeModel>(img =>
                    !img.Id.Equals(Guid.Empty) && img.LeadId == NewLead.LeadId && img.Type == LeadType.Sketch)).FirstOrDefault();
                var resultSaveImgFile = await FileService.SaveTypeFile(eventName: EventModel.Name, type: LeadType.Sketch,
                    byteImage: data.ToArray(), leadTypeModel: imageFiles);

                if (resultSaveImgFile.IsSaved)
                {
                    var leadType = new LeadTypeModel()
                    {
                        Type = resultSaveImgFile.Type,
                        FileName = resultSaveImgFile.FileName,
                        FilePath = resultSaveImgFile.LocalPath,
                        //ImageLink = ScannerResult,
                        LeadId = NewLead.LeadId,
                        CreateDate = resultSaveImgFile.CreateDate
                    };
                    NewLead.SketchBytes = data.ToArray();
                    NewLead.LeadTypesList.Add(leadType);
                    SqLiteService.Delete(imageFiles);
                    SqLiteService.Insert(leadType);
                }
            }
        }

        #endregion

        #region Save XML File

        private async Task SaveXmlFile()
        {
            var xmlFiles = (SqLiteService.GetList<LeadTypeModel>(xml =>
                !xml.Id.Equals(Guid.Empty) && xml.LeadId == NewLead.LeadId && xml.Type == LeadType.Xml)).FirstOrDefault();
            var resultSaveXmlFile = await FileService.SaveTypeFile(eventName: EventModel.Name, type: LeadType.Xml,
                content: NewLead.XmlForm, leadTypeModel: xmlFiles);

            if (resultSaveXmlFile.IsSaved)
            {
                var leadType = new LeadTypeModel()
                {
                    Type = resultSaveXmlFile.Type,
                    FileName = resultSaveXmlFile.FileName,
                    FilePath = resultSaveXmlFile.LocalPath,
                    LeadId = NewLead.LeadId,
                    CreateDate = resultSaveXmlFile.CreateDate
                };

                NewLead.LeadTypesList.Add(leadType);
                SqLiteService.Delete<LeadTypeModel>(xmlFiles);
                SqLiteService.Insert(leadType);
            }
        }

        #endregion

        #region SaveSketchCommand

        public ICommand SaveSketchCommand { get; }
        public async void SaveSketchExecute()
        {
            SavedSignature = CurrentSignature.ToArray();

            var a = SavedSignature;
            var b = a.Select(x => $"{x.X.ToString("G17", CultureInfo.InvariantCulture)},{x.Y.ToString("G17", CultureInfo.InvariantCulture)}").ToArray();

            var s = string.Join(";", b);
            if (!string.IsNullOrEmpty(NewLead.StringSketchPoints))
            {
                NewLead.StringSketchPoints = s + ";0,0;" + NewLead.StringSketchPoints;
            }
            else
            {
                NewLead.StringSketchPoints = s;
            }
            await SaveSketchToJpegImage();
        }

        public async Task<IEnumerable<Point>> LoadPoint()
        {
            if (!string.IsNullOrEmpty(NewLead.StringSketchPoints))
            {
                var points = NewLead.StringSketchPoints.Split(';').Select(x => x.Split(','))
                  .Select(y => new Point(double.Parse(y[0], CultureInfo.InvariantCulture), double.Parse(y[1], CultureInfo.InvariantCulture)))
                  .ToArray();

                CurrentSignature = points;
                return CurrentSignature;
            }

            return null;
        }

        public delegate Task<byte[]> MyEventAction(IEnumerable<Point> point);
        public event MyEventAction MyEvent;
        public async Task SaveSketchToJpegImage()
        {
            var imageByte = await MyEvent?.Invoke(CurrentSignature.ToArray());
            if (imageByte != null)
            {
                var imageFiles = (SqLiteService.GetList<LeadTypeModel>(img =>
                    !img.Id.Equals(Guid.Empty) && img.LeadId == NewLead.LeadId && img.Type == LeadType.Sketch)).FirstOrDefault();
                var resultSaveImgFile = await FileService.SaveTypeFile(eventName: EventModel.Name, type: LeadType.Sketch,
                    byteImage: imageByte, leadTypeModel: imageFiles);

                if (resultSaveImgFile.IsSaved)
                {
                    var leadType = new LeadTypeModel()
                    {
                        Type = resultSaveImgFile.Type,
                        FileName = resultSaveImgFile.FileName,
                        FilePath = resultSaveImgFile.LocalPath,
                        //ImageLink = ScannerResult,
                        LeadId = NewLead.LeadId,
                        CreateDate = resultSaveImgFile.CreateDate
                    };
                    NewLead.LeadTypesList.Add(leadType);
                    SqLiteService.Delete(imageFiles);
                    SqLiteService.Insert(leadType);
                }
            }
        }


        #endregion

        #endregion

        #region Status of lead

        private StatusOfLeadModel StatusIconOfLeadModel()
        {
            return (DataSideMenu.Any(item => item.ItemStatus == SideMenuItemStatus.Warning)) ? StatusOfLeadModel.QuestionMark
                : (DataSideMenu.All(item => item.ItemStatus == SideMenuItemStatus.Full)) ? StatusOfLeadModel.CheckedCircle
                //: (DataSideMenu.All(item => item.ItemStatus != SideMenuItemStatus.None)) ? StatusOfLeadModel.HalfCircle
                : (DataSideMenu.Any(item => item.ItemStatus != SideMenuItemStatus.None)) ? StatusOfLeadModel.HalfCircle : StatusOfLeadModel.EmptyCircle;
        }

        #endregion

        #region CheckSideMenuItemStatus

        public SideMenuItemStatus CheckSideMenuItemStatus(int preIndex)
        {
            if (NewLead.IsLeadWithCard && DataSideMenu[preIndex].Name == "visitor" &&
                DataSideMenu[preIndex].Short == "visitor" && !Template.Pages[preIndex].LeadWithCard.Visible)
            {
                var dataBussiness = SqLiteService.GetList<LeadTypeModel>(lead =>
                        !lead.Id.Equals(Guid.Empty) && lead.Type == LeadType.BusinessCard &&
                        lead.LeadId == NewLead.LeadId)
                    .Any();
                ////if (LeadTypeFiles.Any(type => type.Type == LeadType.BusinessCard))

                if (dataBussiness)
                    return SideMenuItemStatus.Full;
                return SideMenuItemStatus.Warning;
            }

            if (DataSideMenu[preIndex].IsMissed)
            {
                return SideMenuItemStatus.None;
            }

            foreach (var box in Template.Pages[preIndex].Boxs)
            {
                if (box != null && box.Body != null)
                    box.BoxStatus = CheckSideMenuItemStatusInBox(box: box);
            }

            ShowStatusBox(preIndex: preIndex);

            if ((Template.Pages[index: preIndex].Boxs).Any(box => box.BoxStatus == SideMenuItemStatus.Warning && box.Visible))
                return SideMenuItemStatus.Warning;

            if (Template.Pages[index: preIndex].Boxs.All(box => box.BoxStatus == SideMenuItemStatus.None && box.Visible))
                return SideMenuItemStatus.None;

            if ((Template.Pages[index: preIndex].Boxs.Count(box => box.BoxStatus == SideMenuItemStatus.None && box.Visible) +
                 Template.Pages[index: preIndex].Boxs.Count(box => box.BoxType == BoxType.HintAddress && box.Visible) ==
                 Template.Pages[index: preIndex].Boxs.Count(box => box.Visible)) && Template.Pages[index: preIndex].Boxs.Count(box => box.Visible) != 0)
            {
                return SideMenuItemStatus.None;
            }

            // Todo: Sprint 2019 - 24: Update requirement
            //if (Template.Pages[index: preIndex].Boxs.Any(box => box.BoxStatus == SideMenuItemStatus.Half) ||
            //    Template.Pages[index: preIndex].Boxs.Any(box => box.BoxStatus == SideMenuItemStatus.None))
            //    return SideMenuItemStatus.Half;

            if (Template.Pages[index: preIndex].Boxs.Any(box => box.BoxStatus == SideMenuItemStatus.None && box.Visible))
                return SideMenuItemStatus.Half;

            return SideMenuItemStatus.Full;
        }

        #region CheckSideMenuItemStatusInBox

        public SideMenuItemStatus CheckSideMenuItemStatusInBox(Box box)
        {
            var totalCheckboxesUnselected = 0;
            var totalCheckboxesSelected = 0;
            var totalCheckboxesNeedToSelectedMinimize = 0;

            var totalRadioButtonsUnselected = 0;
            var totalRadioButtonsSelected = 0;
            var totalRadioButtonsNeedToSelectedMinimize = 0;

            var totalInputsEmpty = 0;

            var totalInputs = 0;

            var isEmailValid = true;
            var isAutoCompleteValid = true;
            var checkboxInputChildrenEntered = true;

            foreach (var row in box.Body.Rows)
            {
                var checkboxes = row.Inputs.Where(x => x.InputType == InputType.checkbox.ToString()).ToList();
                var checkboxSelected = checkboxes.Where(x => x.Value == true.ToString()).ToList();
                var checkboxUnSelected = checkboxes.Where(x => x.Value == false.ToString()).ToList();

                var radioButtons = row.Inputs.Where(x => x.InputType == InputType.radio.ToString()).ToList();
                var radioButtonsSelected = radioButtons.Where(x => x.Value == true.ToString()).ToList();
                var radioButtonsUnSelected = radioButtons.Where(x => x.Value == false.ToString()).ToList();

                var inputControls = row.Inputs.Where(x =>
                    x.InputType != InputType.checkbox.ToString() && x.InputType != InputType.radio.ToString()).ToList();


                if(inputControls.Any(x => x.Mandatory && string.IsNullOrEmpty(x.Value))) 
                    return SideMenuItemStatus.Warning;
                var inputControlsNotEntered = inputControls.Where(x => string.IsNullOrEmpty(x.Value) && x.Visible).ToList();

                var companyNameInput = inputControls
                    .FirstOrDefault(inputType =>
                        (String.Compare(inputType.InputId, BoxType.Company, StringComparison.OrdinalIgnoreCase) == 0) &&
                        (String.Compare(box.BoxType, BoxType.Company, StringComparison.OrdinalIgnoreCase) == 0));
                if (companyNameInput != null)
                    NewLead.CompanyName = companyNameInput?.Value;

                totalCheckboxesUnselected += checkboxUnSelected.Count;
                totalCheckboxesSelected += checkboxSelected.Count;
                totalCheckboxesNeedToSelectedMinimize += checkboxes.Any() ? 1 : 0;

                totalRadioButtonsUnselected += radioButtonsUnSelected.Count;
                totalRadioButtonsSelected += radioButtonsSelected.Count;
                totalRadioButtonsNeedToSelectedMinimize += radioButtons.Any() ? 1 : 0;

                totalInputsEmpty += inputControlsNotEntered.Count;

                totalInputs += checkboxes.Count + radioButtons.Count + inputControls.Count;

                if (row.Inputs.Any(x => x.InputId == "email"))
                {
                    var inputEmail = row.Inputs.FirstOrDefault(x => x.InputId == "email");
                    if (!string.IsNullOrEmpty(inputEmail?.Value))
                    {
                        isEmailValid = Regex.IsMatch(inputEmail.Value, SdkKeyConstants.EmailRegex,
                            RegexOptions.IgnoreCase);
                    }
                }

                if (row.Inputs.Any(x => x.InputType == "auto"))
                {
                    var inputAutoCompleteList = row.Inputs.Where(x => x.InputType == "auto");
                    foreach (var inputAutoComplete in inputAutoCompleteList)
                    {
                        if (!string.IsNullOrEmpty(inputAutoComplete?.Value))
                        {
                            var data = box.Data.Where(x => x.DataId == inputAutoComplete.InputData).First();
                            if (data != null)
                            {
                                if (data.LanguageItem != null)
                                {
                                    var dataSource = data.LanguageItem.GeDatatLanguages(indexOfLanguage: IndexOfLanguage);
                                    if (!dataSource.Any())
                                    {
                                        isAutoCompleteValid = true;
                                        continue;
                                    }
                                    isAutoCompleteValid = dataSource.Contains(inputAutoComplete.Value);
                                }
                                else
                                {
                                    isAutoCompleteValid = false;
                                }
                            }
                        }
                    }
                }

                if (checkboxes.Any(x => x.InputChildren != null && x.Value == true.ToString()))
                    checkboxInputChildrenEntered = checkboxes.Any(x =>
                        x.InputChildren != null && x.Value == true.ToString() &&
                        !string.IsNullOrEmpty(x.InputChildren.Value));
            }

            // New requirement: <mandatoryAll> property for box 29.09.2020
            if (box.MandatoryAllFields)
            {
                // Full box
                if ((totalInputsEmpty == 0) && (totalCheckboxesSelected >= totalCheckboxesNeedToSelectedMinimize) &&
                    (totalRadioButtonsSelected >= totalRadioButtonsNeedToSelectedMinimize) && isEmailValid && isAutoCompleteValid && checkboxInputChildrenEntered)
                {
                    return SideMenuItemStatus.Full;
                }
                else
                {
                    return SideMenuItemStatus.Warning;
                }
            }
            else
            {
                //if (box.BoxType == BoxType.HintAddress)
                //{
                //    return SideMenuItemStatus.None;
                //}

                // Full box
                if ((totalInputsEmpty == 0) && (totalCheckboxesSelected >= totalCheckboxesNeedToSelectedMinimize) &&
                    (totalRadioButtonsSelected >= totalRadioButtonsNeedToSelectedMinimize) && isEmailValid && isAutoCompleteValid && checkboxInputChildrenEntered)
                {
                    return SideMenuItemStatus.Full;
                }

                if ((totalInputsEmpty == 0) && (totalCheckboxesSelected >= totalCheckboxesNeedToSelectedMinimize) &&
                    (totalRadioButtonsSelected >= totalRadioButtonsNeedToSelectedMinimize) && isEmailValid && isAutoCompleteValid && !checkboxInputChildrenEntered)
                {
                    return SideMenuItemStatus.Half;
                }

                // Todo: Fix Change request sprint 05.02.2019
                if (((totalInputs == (totalCheckboxesUnselected + totalRadioButtonsUnselected + totalInputsEmpty)) &&
                     box.Mandatory) || !isEmailValid || !isAutoCompleteValid)
                {
                    return SideMenuItemStatus.Warning;
                }

                if (totalInputs == (totalCheckboxesUnselected + totalRadioButtonsUnselected + totalInputsEmpty))
                    return SideMenuItemStatus.None;
                
                return SideMenuItemStatus.Half;
            }
        }

        #endregion

        #region ShowStatusBox

        private void ShowStatusBox(int preIndex)
        {
            #region Solution 1 - Comment Code

            /*for (var indexBox = 0; indexBox < Template.Pages[preIndex].Boxs.Count; indexBox++)
            {
                var statusBox = Template.Pages[preIndex].Boxs[indexBox].BoxStatus;

                if (Template.Pages[preIndex].Boxs[indexBox].Size == SizeType.Half.ToString().ToLower())
                {
                    var index = indexBox;
                    var totalChildrensInPage = ((StackLayout) LayoutPages[preIndex]).Children.Count;
                    var maxBox =
                        ((Grid) (((StackLayout) LayoutPages[preIndex]).Children[
                            (indexBox > (totalChildrensInPage - 1)) ? totalChildrensInPage - 1 : indexBox])).Children.Count;
                    for (var indexHalfBox = 0; indexHalfBox < maxBox; indexHalfBox++)
                    {
                        if (Template.Pages[preIndex].Boxs[index].BoxStatus == SideMenuItemStatus.Warning)
                            ((Grid)(((StackLayout)LayoutPages[preIndex]).Children[(indexBox > (totalChildrensInPage - 1)) ? totalChildrensInPage - 1 : indexBox]))
                                .Children[indexHalfBox].BackgroundColor = Color.FromHex("#C8193C");
                        index++;
                    }

                    indexBox = --index;
                }
                else if (statusBox == SideMenuItemStatus.Warning)
                    ((StackLayout)LayoutPages[preIndex]).Children[indexBox].BackgroundColor =
                        Color.FromHex("#C8193C");
            }*/

            #endregion

            try
            {
                var indexBoxInPage = 0;
                var totalChildrensInPage = ((StackLayout)LayoutPages[index: preIndex]).Children.Count;
                for (var indexChildren = 0; indexChildren < totalChildrensInPage - 1; indexChildren++)
                {
                    if ((((StackLayout)LayoutPages[preIndex]).Children[indexChildren]).GetType().Name != ControlType.CrossButton)
                    {
                        var totalChildrenBoxes =
                            ((Grid)(((StackLayout)LayoutPages[preIndex]).Children[indexChildren])).Children.Count;
                        for (var indexChildrenBox = 0; indexChildrenBox < totalChildrenBoxes; indexChildrenBox++)
                        {
                            if (Template.Pages[preIndex].Visible)
                            {
                                if (Template.Pages[preIndex].Boxs[indexBoxInPage].BoxStatus == SideMenuItemStatus.Warning)
                                {
                                    if (Template.Pages[preIndex].Boxs[indexBoxInPage].Size == SizeType.Half.ToString().ToLower())
                                        ((Grid)(((StackLayout)LayoutPages[preIndex]).Children[indexChildren]))
                                            .Children[indexChildrenBox].BackgroundColor = (Color)App.Current.Resources["RedColor"];
                                    else
                                        ((StackLayout)LayoutPages[preIndex]).Children[indexChildren].BackgroundColor = (Color)App.Current.Resources["RedColor"];

                                }
                                else
                                {
                                    if (Template.Pages[preIndex].Boxs[indexBoxInPage].Size == SizeType.Half.ToString().ToLower())
                                        ((Grid)(((StackLayout)LayoutPages[preIndex]).Children[indexChildren]))
                                            .Children[indexChildrenBox].BackgroundColor = (Color)App.Current.Resources["DimGrayColor"];
                                    else
                                        ((StackLayout)LayoutPages[preIndex]).Children[indexChildren].BackgroundColor =
                                            (Color)App.Current.Resources["DimGrayColor"];
                                }
                            }

                            indexBoxInPage++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #endregion

        #region GetStatusSideMenu

        public void GetStatusSideMenu()
        {
            for (int index = 0; index < DataSideMenu.Count; index++)
            {
                DataSideMenu[index].ItemStatus = CheckSideMenuItemStatus(preIndex: index);
            }

            DataSideMenu = new ObservableCollection<SideMenu>(DataSideMenu);
        }

        #endregion

        #region BackCommand

        protected override async Task BackExecute()
        {
            await CheckBusy(async () =>
            {
                //if (StatusLead == StatusOfLeadModel.CreateLead)
                //{
                await ConfirmPopup.Instance.Show(
                TranslateExtension.Get("Confirm"),
                TranslateExtension.Get("DiscardThisLead"),
                TranslateExtension.Get("No"),
                acceptButtonText: TranslateExtension.Get("Yes"),
                acceptCommand: new Command(async () =>
                {
                    await Navigation.GoBackAsync();
                }));
                // }
            });
        }

        #endregion

        #region BackButtonPress

        /// <summary>
        /// //false is default value when system call back press
        /// </summary>
        /// <returns></returns>
        public override bool OnBackButtonPressed()
        {
            if (PopupNavigation.Instance.PopupStack.Count == 1)
            {
                PopupNavigation.Instance.PopAsync();
            }
            else
            {
                Task.Run(async () =>
                {
                    //await CheckBusy(async () =>
                    //{
                        await BackExecute();
                    //});
                });
            }

            return true;
        }

        #endregion

        #region DeleteBusinessCardExecute

        public async void DeleteBusinessCardExecute(string leadTypeId)
        {
            var index = GetIndexLeadType(leadTypeId: leadTypeId);

            var leadType = LeadTypeFiles[index: index];

            await ConfirmPopup.Instance.Show(
                    TranslateExtension.Get("DeleteDocument"),
                    TranslateExtension.Get("DeleteDocumentContent"),
                    TranslateExtension.Get("Cancel"),
                    acceptButtonText: TranslateExtension.Get("Delete"),
                    acceptCommand: new Command(async () =>
                    {
                        await LoadingPopup.Instance.Show(TranslateExtension.Get("Deleting"));

                        FileService.DeleteFile($"{leadType.FilePath}");
                        switch (LeadType)
                        {
                            case LeadType.BusinessCard:
                                NewLead.BusinessCardCountCreated--;
                                break;

                            case LeadType.Attachment:
                                NewLead.IsAttachmentCreated = false;
                                break;

                            case LeadType.Object:
                                NewLead.IsObjectCreated = false;
                                break;

                        }

                        DeleteLeadTypeFile(leadFile: leadType);

                        await Task.Delay(700);
                        await LoadingPopup.Instance.Hide();

                    }));
        }

        #endregion

        #region GetIndexLeadType

        private int GetIndexLeadType(string leadTypeId)
        {
            foreach (var leadType in LeadTypeFiles)
            {
                if (leadType.StringId.Equals(leadTypeId))
                {
                    return LeadTypeFiles.IndexOf(leadType);
                }
            }

            return -1;
        }

        #endregion

        #region VisibleSideMenu

        private bool _isVisibleSideMenu = Device.Idiom != TargetIdiom.Phone;
        public bool IsVisibleSideMenu
        {
            get => _isVisibleSideMenu;
            set => SetProperty(ref _isVisibleSideMenu, value);
        }

        private bool _isVisibleDoNothing;
        public bool IsVisibleDoNothing
        {
            get => _isVisibleDoNothing;
            set => SetProperty(ref _isVisibleDoNothing, value);
        }

        public ICommand IsVisibleSideMenuCommand { get; }
        public async Task IsVisibleSideMenuExecute(string swipeDirection)
        {
            await CheckBusy(async () =>
            {
                switch (swipeDirection)
                {
                    case "Left":
                        // Handle the swipe
                        break;
                    case "Right":
                        // Handle the swipe
                        break;
                    case "Up":
                        // Handle the swipe
                        break;
                    case "Down":
                        // Handle the swipe
                        break;
                }
            });
        }

        #endregion

        #region ContinueButtonHandler

        public static event EventHandler<EventArgs> ContinueHandler;
        public async Task ContinueButtonHandler()
        {
            await CheckBusy(async () =>
            {
                var statusPage = CheckSideMenuItemStatus(preIndex: _preIndexPage);
                if (statusPage == SideMenuItemStatus.Warning)
                {
                    ShowStatusBox(preIndex: _preIndexPage);
                }
                else
                    ContinueHandler?.Invoke(null, EventArgs.Empty);
            });
        }

        #endregion

        #region FinishButtonHandler

        public static event EventHandler<EventArgs> FinishHandler;
        public async Task FinishButtonHandler()
        {
            SaveExcute();
        }

        #endregion

        #region SwitchCardButtonHandler

        private View _visitorPage;
        public View VisitorPage
        {
            get => _visitorPage;
            set => SetProperty(ref _visitorPage, value);
        }
        public static event EventHandler<EventArgs> SwitchCardHandler;

        public ICommand SwitchCardButtonHandlerCommand { get; set; }
        public async Task SwitchCardButtonHandler()
        {
            await CheckBusy(async () =>
            {
                NewLead.IsLeadWithCard = !NewLead.IsLeadWithCard;

                if (NewLead.IsLeadWithCard)
                {
                    LayoutPages[_preIndexPage] = new LeadWithCardContentView(isMissedVisitorPage: false);
                    SwitchCardHandler?.Invoke(LayoutPages[_preIndexPage], EventArgs.Empty);
                }
                else
                {
                    LayoutPages[_preIndexPage] = VisitorPage;
                    SwitchCardHandler?.Invoke(VisitorPage, EventArgs.Empty);
                }

            });
        }

        #endregion

        #region ScanQrCodeButtonHandler
        public static event EventHandler<EventArgs> ScanQrCodeHandler;

        public ICommand ScanQrCodeButtonHandlerCommand { get; set; }
        public async Task ScanQrCodeButtonHandler()
        {
            await CheckBusy(async () =>
            {
                if(Device.RuntimePlatform == Device.iOS)
                {
                    // Scan QR Code Page for iOS
                    await Navigation.NavigateAsync(PageManager.ScanQrCodeiOSPage);
                }
                else
                {
                    // Scan QR Code Page for Android
                    await Navigation.NavigateAsync(PageManager.ScanQrCodePage);
                }
            });
        }

        #endregion

        #region HiddenTopMenu

        public ICommand HiddenTopMenuCommand { get; set; }
        private async void HiddenTopMenuExcute()
        {
            await CheckBusy(async () =>
            {
                IsShownTopMenu = !IsShownTopMenu;
            });
        }

        #endregion

        #region InvokeEventToShowImageInContentView

        public void InvokeEventToShowImageInContentView()
        {
            ImageView?.Invoke(ScannerResult[Position], null);
        }

        #endregion
    }
}
