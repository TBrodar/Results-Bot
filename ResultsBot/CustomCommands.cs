using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ResultsBot
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand StartPause = new RoutedUICommand
                        (
                                "Start/Pause on Checkpoint",
                                "Start/Pause on Checkpoint",
                                typeof(CustomCommands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.F9)
                                }
                        );
        public static readonly RoutedUICommand ShowMousePositionAndPixelColor = new RoutedUICommand
                       (
                               "Show mouse position and pixel color on mouse position.",
                               "Show mouse position and pixel color on mouse position.",
                               typeof(CustomCommands),
                               new InputGestureCollection()
                               {
                                        new KeyGesture(Key.F6)
                               }
                       );
        public static readonly RoutedUICommand SetF1Position = new RoutedUICommand
                (
                        "Set [x,y] position",
                        "Set [x,y] position",
                        typeof(CustomCommands),
                        new InputGestureCollection()
                        {
                                        new KeyGesture(Key.F1)
                        }
                );
        public static readonly RoutedUICommand SetF2Position = new RoutedUICommand
                (
                        "Set [x1,y1] position",
                        "Set [x1,y1] position",
                        typeof(CustomCommands),
                        new InputGestureCollection()
                        {
                                        new KeyGesture(Key.F2)
                        }
                );
        public static readonly RoutedUICommand SetF3Position = new RoutedUICommand
        (
                "Set [x2,y2] position",
                "Set [x2,y2] position",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                                        new KeyGesture(Key.F3)
                }
        );
        public static readonly RoutedUICommand CapturePixelColor = new RoutedUICommand
(
        "Capture [x2,y2] pixel color",
        "Capture [x2,y2] pixel color",
        typeof(CustomCommands),
        new InputGestureCollection()
        {
                                        new KeyGesture(Key.F5)
        }
);

        public static readonly RoutedUICommand CaptureImageFromX1Y1X2Y2Rectangle = new RoutedUICommand
                (
                        "Capture image from [x1,y1],[x2,y2] position",
                        "Capture image from [x1,y1],[x2,y2] position",
                        typeof(CustomCommands),
                        new InputGestureCollection()
                        {
                                        new KeyGesture(Key.F4)
                        }
                );
    }
}
