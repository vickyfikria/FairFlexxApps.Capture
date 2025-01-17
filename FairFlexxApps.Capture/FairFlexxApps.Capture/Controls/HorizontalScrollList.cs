﻿using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.Maui;

namespace FairFlexxApps.Capture.Controls
{
    public class HorizontalScrollList : ScrollView
    {
        public HorizontalScrollList()
        {
            this.Orientation = ScrollOrientation.Horizontal;
        }

        #region ItemSource

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
           "ItemsSource",
           typeof(IEnumerable),
           typeof(HorizontalScrollList),
           null,
           BindingMode.OneWay,
           propertyChanged: (bindable, value, newValue) =>
           {
               var obs = value as INotifyCollectionChanged;
               var self = (HorizontalScrollList)bindable;
               if (obs != null)
                   obs.CollectionChanged -= self.HandleItemChanged;

               self.Populate();

               obs = newValue as INotifyCollectionChanged;
               if (obs != null)
                   obs.CollectionChanged += self.HandleItemChanged;
           });

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)this.GetValue(ItemsSourceProperty);
            set => this.SetValue(ItemsSourceProperty, value);
        }

        #endregion

        #region ItemTemplate

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(HorizontalScrollList),
            null,
            propertyChanged: (bindable, value, newValue) => ((HorizontalScrollList)bindable).Populate());



        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)this.GetValue(ItemTemplateProperty);
            set => this.SetValue(ItemTemplateProperty, value);
        }

        #endregion
            
        #region Show UI

        private bool willUpdate = true;
        private void HandleItemChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (!willUpdate)
            {
                willUpdate = true;
                Device.BeginInvokeOnMainThread(Populate);
            }
        }

        private void Populate()
        {
            willUpdate = false;

            Content = null;

            if (ItemsSource == null || ItemTemplate == null)
            {
                return;
            }

            var list = new StackLayout { Orientation = StackOrientation.Horizontal };

            foreach (var viewModel in ItemsSource)
            {
                var content = ItemTemplate.CreateContent();
                if (!(content is View) && !(content is ViewCell))
                {
                    throw new Exception($"Invalid visual object {nameof(content)}");
                }

                var view = content is View ? content as View : ((ViewCell)content).View;
                view.BindingContext = viewModel;

                list.Children.Add(view);
            }

            if (list.Children.Count == 0)
            {
                list.Children.Add(new Label
                {
                    HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true),
                    VerticalOptions = new LayoutOptions(LayoutAlignment.Fill, true),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    BackgroundColor = new Color(0, 0, 0, 0.2),
                    Text = "No button"
                });
            }

            Content = list;
        }

        #endregion

    }
}
