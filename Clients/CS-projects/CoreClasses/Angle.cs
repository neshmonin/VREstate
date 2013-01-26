using System;

namespace CoreClasses
{
    public static class Angle
    {
        public static double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180d) * degrees;
        }

        public static double RadiansToDegrees(double radians)
        {
            return (180d / Math.PI) * radians;
        }

        public static int As180(int angle)
        {
            angle = angle % 360;

            angle = (angle < -180) ?
                        180 - Math.Abs(angle % 180)
                        : (angle > 180) ?
                            Math.Abs(angle % 180) - 180
                            : angle;
            return angle;
        }

        public static double Heading_dFromMatrix(double [][] matrix)
        {
            double angle_d = 0.0f;
            double heading0 = Math.Round(RadiansToDegrees(Math.Acos(Math.Round(matrix[0][0],3))), 2);
            double heading1 = Math.Round(RadiansToDegrees(Math.Asin(Math.Round(matrix[1][0],3))), 2);
            if (heading0 > 0 == heading1 > 0)
            {
                if (Math.Sign(heading0) == Math.Sign(heading1))
                {   //    N A  * red
                    //      | /
                    //      |/ 
                    //  <---+----->
                    //  W   |     E
                    //    S V
                    angle_d = (90 - heading0);
                }
                else
                {   //    N A
                    //      |
                    //  <---+----->
                    //  W   |\    E
                    //      | \
                    //    S V  * red
                    angle_d = (90 + heading0);
                }
            }
            else
            {
                if (Math.Sign(heading0) == Math.Sign(heading1))
                {   // red *  A N
                    //      \ |
                    //       \|
                    //  <-----+--->
                    //  W     |   E
                    //      S V
                    angle_d = heading1 + 270;
                }
                else
                {   //        A N
                    //        |
                    //  <-----+--->
                    //  W    /|   E
                    //      / |
                    // red *  V S
                    angle_d = heading0 + 90;
                }
            }
            return angle_d;
        }
    }

}
