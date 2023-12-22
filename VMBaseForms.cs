using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Controls;

namespace Core
{
    public interface IFormsRegistrator
    {
        void Register(IFormsServer fs);
    }
    public class FormsRegistrator<T> : IFormsRegistrator
        where T : VMBaseForms
    {
        public void Register(IFormsServer fs)
        {
            fs.RegisterModelView(typeof(T).Name, VMBase.ServiceProvider.GetRequiredService<T>);
        }
    }

    public interface IFormsServer
    {
        /// <summary>
        /// регистрируем генератор модели представления
        /// </summary>
        /// <param name="RootContentID"> RootContentID.AnyData.AnyData...) </param>
        /// <param name="RegFunc">генератор модели представления</param>
        void RegisterModelView(string RootContentID, Func<VMBaseForms> RegFunc);
        /// <summary>
        /// работает если для ContentID (RootContentID) зарегистрирована RegisterModelView RegFunc фабрика окна
        /// </summary>
        /// <param name="ContentID"> ContentID= RootContentID.AnyData.AnyData...</param>
        /// <returns></returns>
        VMBaseForms AddOrGet(string ContentID);
        // низкоуровневые вункции
        VMBaseForms? Contains(string ContentID);
        VMBaseForms Add(VMBaseForms vmbase);
        void Remove(VMBaseForms RemForm);
    }
    /// <summary>
    /// <para>
    /// ОСНОВНАЯ ИДЕЯ ПРОГРАММЫ:
    /// </para>
    /// Каждое окно создает при активации свои кнопки управления и меню !
    /// <para>
    /// событие OnMenuActivate: создать tools, menus ,вручную добавить в DynamicItems,
    /// IToolServer, IMenuItemServer
    /// </para>
    /// <para>
    /// событие OnMenuDeActivate: обнулить внутренние ссылки, если есть, на tools, menus.
    /// Очищать DynamicItems и удалять tools, menus из IToolServer,IMenuItemServer не надо, 
    /// будет удалено автоматически
    /// </para>
    /// </summary>
    public abstract class VMBaseForms : VMBase
    {               
        //public static void Register<T>(string contentID) where T : VMBaseForms
        //{
        //    var fs = ServiceProvider.GetRequiredService<IFormsServer>();
        //    fs.RegisterModelView(contentID, ServiceProvider.GetRequiredService<T>);
        //}
        public static VMBaseForms CreateAndShow(string ID)
        {
            var fs = ServiceProvider.GetRequiredService<IFormsServer>();
            var pvm = fs.AddOrGet(ID);
            pvm.IsVisible = true;
            pvm.Focus();
            pvm.IsActive = true;
            pvm.IsSelected = true;
            return pvm;            
        }

        //public delegate void OnCloseHandler(object sender);
        //public static event OnCloseHandler? OnClose;

        #region Dynamic Tools And Menus 
        protected List<PriorityItemBase> DynamicItems = new List<PriorityItemBase>();
        private DispatcherTimer _Activatetimer;
        private DispatcherTimer _DeActivatetimer;
        private void ActivateMenu()
        {
            _DeActivatetimer.Stop();
            _Activatetimer.Start();
        }
        private void DeActivateMenu()
        {
            _Activatetimer.Stop();
            _DeActivatetimer.Start();
        }

        public VMBaseForms()
        {
            _Activatetimer = new();
            _Activatetimer.Interval = new(TimeSpan.TicksPerMillisecond * 500);
            _Activatetimer.Tick += (s, e) => 
            { 
                _Activatetimer.Stop(); 
                if (DynamicItems.Count ==0) OnMenuActivate?.Invoke(); 
            };

            _DeActivatetimer = new();
            _DeActivatetimer.Interval = new(TimeSpan.TicksPerMillisecond * 100);
            _DeActivatetimer.Tick += (s, e) => 
            { 
                _DeActivatetimer.Stop();
                if (DynamicItems.Count > 0) OnMenuDeActivate?.Invoke();
                if (DynamicItems.Count > 0)
                {
                    IToolServer ts = ServiceProvider.GetRequiredService<IToolServer>();
                    IMenuItemServer ms = ServiceProvider.GetRequiredService<IMenuItemServer>();
                    ts.Remove(DynamicItems.OfType<ToolButton>());
                    ms.Remove(DynamicItems.OfType<MenuItemVM>());
                    DynamicItems.Clear();
                }
            };

            OnActivate += ActivateMenu;
            OnDeActivate += DeActivateMenu;
        }
        #endregion

        #region Close
        public void Close()
        {
            var fs = ServiceProvider.GetRequiredService<IFormsServer>();
            fs.Remove(this);
        }
        RelayCommand _closeCommand = null!;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(Close, ()=>CanClose);
                }

                return _closeCommand;
            }
        }
        #endregion

        #region Focus
        public delegate void OnFocusHandler(object sender);
        public static event OnFocusHandler? OnFocus;
        public void Focus()
        {
            OnFocus?.Invoke(this);
        }
        #endregion

        #region Title
        private string _title = string.Empty;
        public string Title
        {
            get => _title; set
            {
                if (value != _title)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
        #endregion

        #region IsVisible

        public delegate void OnVisibleChangedHandler(object sender);
        public static event OnVisibleChangedHandler? OnVisibleChanged;
        public static void OnVisibleChange(object sender)
        {
            OnVisibleChanged?.Invoke(sender);
        }

        public bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;

                    if (!_isVisible) DeActivateMenu();
                    else if (_isActive) ActivateMenu();

                    OnPropertyChanged(nameof(IsVisible));
                    OnVisibleChange(this);
                }
            }
        }
        #endregion

        #region Icon
        public Uri? IconSource
        {
            get;
            set;
        } = null;
        public bool IconSourceEnable => IconSource != null;
        #endregion

        #region IsSelected

        protected delegate void SelectHandler();
        protected event SelectHandler? OnSelect;
        protected event SelectHandler? OnDeSelect;
        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    if (_isSelected && !value)
                        OnDeSelect?.Invoke();
                    else
                        OnSelect?.Invoke();

                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        #endregion

        #region IsActive

        protected delegate void ActivateHandler();
        private event ActivateHandler? OnActivate;
        private event ActivateHandler? OnDeActivate;
        protected event ActivateHandler? OnMenuActivate;
        protected event ActivateHandler? OnMenuDeActivate;
        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    if (_isActive && !value) 
                        OnDeActivate?.Invoke();
                    else 
                        OnActivate?.Invoke();

                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        #endregion

        #region FloatingWidth
        public double FloatingWidth { get; set; } = 0.0;

        #endregion

        #region FloatingHeight

        public double FloatingHeight { get; set; } = 0.0;

        #endregion

        #region FloatingLeft

        public double FloatingLeft { get; set; } = 0.0;

        #endregion

        #region FloatingTop

        public double FloatingTop { get; set; } = 0.0;
        #endregion

        //#region IsFloating

        //private bool _isFloating = false;

        //public bool IsFloating
        //{
        //    get
        //    {
        //        return _isFloating;
        //    }
        //    internal set
        //    {
        //        if (_isFloating != value)
        //        {
        //            _isFloating = value;
        //            OnPropertyChanged("IsFloating");
        //        }
        //    }
        //}

        //#endregion

        #region CanClose
        bool _canClose = true;
        public bool CanClose
        { 
            get=> _canClose;
            set
            {
                if (_canClose != value)
                {
                    _canClose = value;
                    OnPropertyChanged(nameof(CanClose));
                }

            }
        }

        #endregion

        #region CanFloat
        public bool CanFloat { get; set; } = true;
        #endregion

        #region ToolTip
        public bool ToolTipEnable=> ToolTip != null;
        public string? ToolTip { get; set; } = null;
        #endregion

    }
    public class DocumentVM : VMBaseForms
    {
        #region Description
        public string? Description { get; set; } = null;

        public bool DescriptionEnable => Description != null;
        #endregion

        #region CanMove
        public bool CanMove { get; set; } = true;
        #endregion

    }
    [Flags]
    public enum ShowStrategy : byte
    {
        Most = 0x0001,
        Left = 0x0002,
        Right = 0x0004,
        Top = 0x0010,
        Bottom = 0x0020,
    }
    public class ToolVM: VMBaseForms
    {
        public ToolVM(): base() 
        {
            CanClose = false;
        }
        #region AutoHideWidth
        public double AutoHideWidth { get; set; } = 200;

        #endregion

        #region AutoHideMinWidth

        public double AutoHideMinWidth { get; set; } = 20;

        #endregion

        #region AutoHideHeight

        public double AutoHideHeight { get; set; } = 200;
        #endregion

        #region AutoHideMinHeight

        public double AutoHideMinHeight { get; set; } = 20;

        #endregion

        #region CanHide

        public bool CanHide { get; set; } = true;

        #endregion

        #region CanAutoHide

        public bool CanAutoHide { get; set; }=true;

        #endregion

        #region CanDockAsTabbedDocument

        public bool CanDockAsTabbedDocument { get; set; } = true;
        #endregion

        #region ShowStrategy
        public ShowStrategy? ShowStrategy { get; set; } = null;
        #endregion
    }
}
