using System;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#elif (WINDOWS_PHONE || NETFX_451)
using System.Windows;
using System.Windows.Media.Animation;
#endif

namespace _2048
{
    class Animation
    {

#if NETFX_CORE
        public static string CreatePropertyPath(string PropertyPath)
        {
            return PropertyPath;
        }
#elif (WINDOWS_PHONE || NETFX_451)
        public static PropertyPath CreatePropertyPath(string PropertyPath)
        {
            return new PropertyPath(PropertyPath);
        }
#endif

        public static DoubleAnimation CreateDoubleAnimation(double? From, double? To, long DurationTicks)
        {
            var animation = new DoubleAnimation();
            animation.From = From;
            animation.To = To;
            animation.Duration = new Duration(new TimeSpan(DurationTicks));
#if NETFX_CORE
            animation.EnableDependentAnimation = true;
#endif
            return animation;
        }
    }
}
