
using FairFlexxApps.Capture.Views.ViewCells.TopMenuViews.TouchTracking;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;

#if ANDROID
using View = Android.Views.View;
#elif IOS
using UIKit;
using CoreGraphics;
using Foundation;
#endif
namespace FairFlexxApps.Capture.Views.ViewCells.TopMenuViews.TouchTracking
{


    public class TouchPlatformEffect : PlatformEffect
    {

        public event TouchActionEventHandler TouchAction;

        public bool Capture { set; get; }

        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }


        protected override void OnAttached()
        {
#if ANDROID
            // Get the Android View corresponding to the Element that the effect is attached to  
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library  
            TouchPlatformEffect touchEffect =
                (TouchPlatformEffect)Element.Effects.
                    FirstOrDefault(e => e is TouchPlatformEffect);

            if (touchEffect != null && view != null)
            {
                viewDictionary.Add(view, this);

                formsElement = Element;

                libTouchEffect = touchEffect;

                // Save fromPixels function  
                fromPixels = view.Context.FromPixels;

                // Set event handler on View  
                view.Touch += OnTouch;
            }

#elif IOS
            // Get the iOS UIView corresponding to the Element that the effect is attached to  
            view = Control == null ? Container : Control;  
  
            // Uncomment this line if the UIView does not have touch enabled by default  
            //view.UserInteractionEnabled = true;  
  
            // Get access to the TouchEffect class in the .NET Standard library  
            TouchPlatformEffect effect = (TouchPlatformEffect)Element.Effects.FirstOrDefault(e => e is TouchPlatformEffect);  
  
            if (effect != null && view != null)  
            {  
                // Create a TouchRecognizer for this UIView  
                touchRecognizer = new TouchRecognizer(Element, view, effect);  
                view.AddGestureRecognizer(touchRecognizer);  
            }  
#endif
        }

        protected override void OnDetached()
        {
#if ANDROID
            if (viewDictionary.ContainsKey(view))
            {
                viewDictionary.Remove(view);
                view.Touch -= OnTouch;
            }
#elif IOS
            if (touchRecognizer != null)  
            {  
                // Clean up the TouchRecognizer object  
                touchRecognizer.Detach();  
  
                // Remove the TouchRecognizer from the UIView  
                view.RemoveGestureRecognizer(touchRecognizer);  
            }  
#endif
        }

#if ANDROID
        View view;
        Element formsElement;
        TouchPlatformEffect libTouchEffect;
        bool capture;
        Func<double, double> fromPixels;
        int[] twoIntArray = new int[2];

        static Dictionary<View, TouchPlatformEffect> viewDictionary =
            new Dictionary<View, TouchPlatformEffect>();

        static Dictionary<int, TouchPlatformEffect> idToEffectDictionary =
            new Dictionary<int, TouchPlatformEffect>();

        void OnTouch(object sender, View.TouchEventArgs args)
        {
            // Two object common to all the events  
            View senderView = sender as View;
            Android.Views.MotionEvent motionEvent = args.Event;

            // Get the pointer index  
            int pointerIndex = motionEvent.ActionIndex;

            // Get the id that identifies a finger over the course of its progress  
            int id = motionEvent.GetPointerId(pointerIndex);


            senderView.GetLocationOnScreen(twoIntArray);
            Point screenPointerCoords = new Point(twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                                  twoIntArray[1] + motionEvent.GetY(pointerIndex));


            // Use ActionMasked here rather than Action to reduce the number of possibilities  
            switch (args.Event.ActionMasked)
            {
                case Android.Views.MotionEventActions.Down:
                case Android.Views.MotionEventActions.PointerDown:
                    FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);

                    idToEffectDictionary.Add(id, this);

                    capture = libTouchEffect.Capture;
                    break;

                case Android.Views.MotionEventActions.Move:
                    // Multiple Move events are bundled, so handle them in a loop  
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (capture)
                        {
                            senderView.GetLocationOnScreen(twoIntArray);

                            screenPointerCoords = new Point(twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                                            twoIntArray[1] + motionEvent.GetY(pointerIndex));

                            FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                        }
                        else
                        {
                            CheckForBoundaryHop(id, screenPointerCoords);

                            if (idToEffectDictionary[id] != null)
                            {
                                FireEvent(idToEffectDictionary[id], id, TouchActionType.Moved, screenPointerCoords, true);
                            }
                        }
                    }
                    break;

                case Android.Views.MotionEventActions.Up:
                case Android.Views.MotionEventActions.Pointer1Up:
                    if (capture)
                    {
                        FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                    }
                    else
                    {
                        CheckForBoundaryHop(id, screenPointerCoords);

                        if (idToEffectDictionary[id] != null)
                        {
                            FireEvent(idToEffectDictionary[id], id, TouchActionType.Released, screenPointerCoords, false);
                        }
                    }
                    idToEffectDictionary.Remove(id);
                    break;

                case Android.Views.MotionEventActions.Cancel:
                    if (capture)
                    {
                        FireEvent(this, id, TouchActionType.Cancelled, screenPointerCoords, false);
                    }
                    else
                    {
                        if (idToEffectDictionary[id] != null)
                        {
                            FireEvent(idToEffectDictionary[id], id, TouchActionType.Cancelled, screenPointerCoords, false);
                        }
                    }
                    idToEffectDictionary.Remove(id);
                    break;
            }
        }

        void CheckForBoundaryHop(int id, Point pointerLocation)
        {
            TouchPlatformEffect touchPlatformEffectHit = null;

            foreach (View view in viewDictionary.Keys)
            {
                // Get the view rectangle  
                try
                {
                    view.GetLocationOnScreen(twoIntArray);
                }
                catch // System.ObjectDisposedException: Cannot access a disposed object.  
                {
                    continue;
                }
                Rect rect = new Rect(twoIntArray[0], twoIntArray[1], view.Width, view.Height);

                if (rect.Contains(pointerLocation))
                {
                    touchPlatformEffectHit = viewDictionary[view];
                }
            }

            if (touchPlatformEffectHit != idToEffectDictionary[id])
            {
                if (idToEffectDictionary[id] != null)
                {
                    FireEvent(idToEffectDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
                }
                if (touchPlatformEffectHit != null)
                {
                    FireEvent(touchPlatformEffectHit, id, TouchActionType.Entered, pointerLocation, true);
                }
                idToEffectDictionary[id] = touchPlatformEffectHit;
            }
        }

        void FireEvent(TouchPlatformEffect touchPlatformEffect, int id, TouchActionType actionType, Point pointerLocation, bool isInContact)
        {
            // Get the method to call for firing events  
            Action<Element, TouchActionEventArgs> onTouchAction = touchPlatformEffect.libTouchEffect.OnTouchAction;

            // Get the location of the pointer within the view  
            touchPlatformEffect.view.GetLocationOnScreen(twoIntArray);
            double x = pointerLocation.X - twoIntArray[0];
            double y = pointerLocation.Y - twoIntArray[1];
            Point point = new Point(fromPixels(x), fromPixels(y));

            // Call the method  
            onTouchAction(touchPlatformEffect.formsElement,
                new TouchActionEventArgs(id, actionType, point, isInContact));
        }


#elif IOS
        UIView view;  
        TouchRecognizer touchRecognizer;
#endif
    }
}

#if IOS
    class TouchRecognizer : UIGestureRecognizer  
    {  
        Element element;        // Forms element for firing events  
        UIView view;            // iOS UIView   
        TouchPlatformEffect touchPlatformEffect;  
        bool capture;  
  
        static Dictionary<UIView, TouchRecognizer> viewDictionary =  
            new Dictionary<UIView, TouchRecognizer>();  
  
        static Dictionary<long, TouchRecognizer> idToTouchDictionary =  
            new Dictionary<long, TouchRecognizer>();  
  
        public TouchRecognizer(Element element, UIView view, TouchPlatformEffect touchPlatformEffect)  
        {  
            this.element = element;  
            this.view = view;  
            this.touchPlatformEffect = touchPlatformEffect;  
  
            viewDictionary.Add(view, this);  
        }  
  
        public void Detach()  
        {  
            viewDictionary.Remove(view);  
        }  
  
        // touches = touches of interest; evt = all touches of type UITouch  
        public override void TouchesBegan(NSSet touches, UIEvent evt)  
        {  
            base.TouchesBegan(touches, evt);  
  
            foreach (UITouch touch in touches.Cast<UITouch>())  
            {  
                long id = ((IntPtr)touch.Handle).ToInt64();  
                FireEvent(this, id, TouchActionType.Pressed, touch, true);  
  
                if (!idToTouchDictionary.ContainsKey(id))  
                {  
                    idToTouchDictionary.Add(id, this);  
                }  
            }  
  
            // Save the setting of the Capture property  
            capture = touchPlatformEffect.Capture;  
        }  
  
        public override void TouchesMoved(NSSet touches, UIEvent evt)  
        {  
            base.TouchesMoved(touches, evt);  
  
            foreach (UITouch touch in touches.Cast<UITouch>())  
            {  
                long id = ((IntPtr)touch.Handle).ToInt64();  
  
                if (capture)  
                {  
                    FireEvent(this, id, TouchActionType.Moved, touch, true);  
                }  
                else  
                {  
                    CheckForBoundaryHop(touch);  
  
                    if (idToTouchDictionary[id] != null)  
                    {  
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Moved, touch, true);  
                    }  
                }  
            }  
        }  
  
        public override void TouchesEnded(NSSet touches, UIEvent evt)  
        {  
            base.TouchesEnded(touches, evt);  
  
            foreach (UITouch touch in touches.Cast<UITouch>())  
            {  
                long id = ((IntPtr)touch.Handle).ToInt64();  
  
                if (capture)  
                {  
                    FireEvent(this, id, TouchActionType.Released, touch, false);  
                }  
                else  
                {  
                    CheckForBoundaryHop(touch);  
  
                    if (idToTouchDictionary[id] != null)  
                    {  
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Released, touch, false);  
                    }  
                }  
                idToTouchDictionary.Remove(id);  
            }  
        }  
  
        public override void TouchesCancelled(NSSet touches, UIEvent evt)  
        {  
            base.TouchesCancelled(touches, evt);  
  
            foreach (UITouch touch in touches.Cast<UITouch>())  
            {  
                long id = ((IntPtr)touch.Handle).ToInt64();  
  
                if (capture)  
                {  
                    FireEvent(this, id, TouchActionType.Cancelled, touch, false);  
                }  
                else if (idToTouchDictionary[id] != null)  
                {  
                    FireEvent(idToTouchDictionary[id], id, TouchActionType.Cancelled, touch, false);  
                }  
                idToTouchDictionary.Remove(id);  
            }  
        }  
  
        void CheckForBoundaryHop(UITouch touch)  
        {  
            long id = ((IntPtr)touch.Handle).ToInt64();  
  
            // TODO: Might require converting to a List for multiple hits  
            TouchRecognizer recognizerHit = null;  
  
            foreach (UIView view in viewDictionary.Keys)  
            {  
                CGPoint location = touch.LocationInView(view);  
  
                if (new CGRect(new CGPoint(), view.Frame.Size).Contains(location))  
                {  
                    recognizerHit = viewDictionary[view];  
                }  
            }  
            if (recognizerHit != idToTouchDictionary[id])  
            {  
                if (idToTouchDictionary[id] != null)  
                {  
                    FireEvent(idToTouchDictionary[id], id, TouchActionType.Exited, touch, true);  
                }  
                if (recognizerHit != null)  
                {  
                    FireEvent(recognizerHit, id, TouchActionType.Entered, touch, true);  
                }  
                idToTouchDictionary[id] = recognizerHit;  
            }  
        }  
  
        void FireEvent(TouchRecognizer recognizer, long id, TouchActionType actionType, UITouch touch, bool isInContact)  
        {  
            // Convert touch location to Xamarin.Forms Point value  
            CGPoint cgPoint = touch.LocationInView(recognizer.View);  
            Point xfPoint = new Point(cgPoint.X, cgPoint.Y);  
  
            // Get the method to call for firing events  
            Action<Element, TouchActionEventArgs> onTouchAction = recognizer.touchPlatformEffect.OnTouchAction;  
  
            // Call that method  
            onTouchAction(recognizer.element,  
                new TouchActionEventArgs(id, actionType, xfPoint, isInContact));  
        }  
    }  
#endif

