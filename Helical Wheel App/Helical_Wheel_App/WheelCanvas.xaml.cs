﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms; 

namespace Helical_Wheel_App
{
    public partial class WheelCanvas : ContentView
    {
        private static int RADIUS = 90;
        private static int AMINORADIUS = 9;
        private static float ScaleFactor = 1.938889f;
        private float WheelEnlarger = 0;
        private string AminoSequence = "";
        public List<KeyValuePair<string, Point>> HelicalStructure {get;set;}
        // specifying the colors for the various options on the graph
        SKPaint Wheel = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,            
        };
        SKPaint Line = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
        };
        SKPaint PolarAminoAcid = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Blue,
            BlendMode = SKBlendMode.SrcATop
        };
        SKPaint NonPolarAminoAcid = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black,
            BlendMode = SKBlendMode.SrcATop

        };
        SKPaint PolarLetters = new SKPaint
        {
            Style = SKPaintStyle.StrokeAndFill,
            Color = SKColors.WhiteSmoke,
            TextScaleX = .5f
        };
        SKPaint NonPolarLetters = new SKPaint
        {
            Style = SKPaintStyle.StrokeAndFill,
            Color = SKColors.White,
            TextScaleX = .5f
        };
        SKPaint InvalidAminoAcid = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Red,
            BlendMode = SKBlendMode.SrcATop
        };
        public WheelCanvas(string aminoSeq, double enlargeVal)
        {
            AminoSequence = aminoSeq;
            HelicalStructure = new List<KeyValuePair<string, Point>>();
            WheelEnlarger = (float)enlargeVal;
            InitializeComponent();

        }
        public void canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var surface = args.Surface;
            // set the canvas
            var canvas = surface.Canvas;
            // set the background color
            canvas.Clear(SKColors.WhiteSmoke);
            //canvasView.HeightRequest = .1 * App.ScreenHeight;
            // set width and height
            var percentDiff = App.ScreenHeight > App.ScreenWidth ? ((App.ScreenHeight / App.ScreenWidth) / ScaleFactor) + WheelEnlarger : ((App.ScreenWidth / App.ScreenHeight) / ScaleFactor) + WheelEnlarger;
            int width = args.Info.Width;
            int height = args.Info.Height;
            // translate the (0,0) point to the middle of the canvas
            canvas.Translate((width / 2), (height / 2));
            //scale i
            int modify = 0;
            int numFactors = 0;
            int incr = 19;
            float percentToMinimize;
            if (AminoSequence.Contains(","))
            {
                modify = AminoSequence.Split(',').Count();
            }
            else
            {
                modify = AminoSequence.ToCharArray().Count();
            }
            for(int i = 1; i<= modify; i++)
            {
                if(i % incr == 0)
                {
                    numFactors += 1;
                    incr += 18;
                }
            }
            percentToMinimize = (float)(1 - (numFactors * .12));
            percentToMinimize = percentToMinimize > .4 ? percentToMinimize : .4f;
            canvas.Scale((width / 200f) * percentDiff * percentToMinimize);
            // drawing the circle at the mid-point
            canvas.DrawCircle(0, 0,RADIUS * percentDiff, Wheel);
            string aminos = AminoSequence;
            // determine if the input is 1 or 3 letter abbreviations
            if (aminos.Contains(","))
                HelicalWheelBuilderThreeLetter(aminos, canvas);
            else
                HelicalWheelBuilderLetter(aminos, canvas);
            
        }
        public void HelicalWheelBuilderLetter(string aminoAcids, SKCanvas canvas)
        {
            // init. my variables
            var aminoClass = new AminoAcids();
            var listAminos = aminoAcids.ToCharArray().ToList();
            float x = 0; float y = 0;
            int angle = 270;
            char lastChar = '0';
            bool polarity = false;
            float xModifier = 0;
            float yModifier = 0;
            HelicalStructure.Clear();
            float modIncrementer = 0;
            float scaleModifier = 19;
            int incr = 1;
            var percentDiff = App.ScreenHeight > App.ScreenWidth ? ((App.ScreenHeight / App.ScreenWidth) / ScaleFactor) + WheelEnlarger: ((App.ScreenWidth / App.ScreenHeight) / ScaleFactor) + WheelEnlarger;
            // iterate over the amino acids
            foreach (var item in listAminos)
            {
                // if its a valid character
                if (char.IsLetter(item))
                {
                    // acheck to make sure to add the 
                    if (lastChar != '0')
                    {
                        //draw line
                        if(incr < 18)
                            canvas.DrawLine(x, y, (float)((RADIUS * percentDiff) * Math.Cos(angle * Math.PI / 180F)), (float)((RADIUS * percentDiff) * Math.Sin(angle * Math.PI / 180F)), Line);
                        // check if polar or not then draw the circle accordingly
                        if (polarity)
                            canvas.DrawText(lastChar.ToString().ToUpper() + incr, x - (6 * percentDiff), y + (4 * percentDiff), PolarLetters);
                        else
                            canvas.DrawText(lastChar.ToString().ToUpper() + incr, x - (6 * percentDiff), y + (4 * percentDiff), NonPolarLetters);
                        incr += 1;
                    }
                    // determine the x & y coordinates using this formula
                    x = (float)((RADIUS * percentDiff) * Math.Cos(angle * Math.PI / 180F));
                    y = (float)((RADIUS * percentDiff) * Math.Sin(angle * Math.PI / 180F));
                    // add x & y coordinates to my global variable
                    HelicalStructure.Add(new KeyValuePair<string, Point>(item.ToString().ToUpper() + incr, new Point(x,y)) );
                    if(incr % scaleModifier == 0)
                    {
                        modIncrementer += 1;
                        scaleModifier = scaleModifier + 18;
                    }
                    xModifier = x > 0? 13f * modIncrementer * percentDiff : -13f * modIncrementer * percentDiff;
                    yModifier = y > 0? 13f * modIncrementer * percentDiff : -13f * modIncrementer * percentDiff;
                    if(Math.Abs(x) < 5 && modIncrementer > 0)
                    {
                        yModifier = yModifier > 0 ? (yModifier - (.5f * percentDiff)) + (6 * modIncrementer * percentDiff) : (yModifier + (.5f * percentDiff)) - (6 * modIncrementer * percentDiff);
                        xModifier = 0;
                    }
                    x += xModifier;
                    y += yModifier;
                    if (aminoClass.IsAminoAcid(null, item))
                    {
                        if (polarity = aminoClass.IsPolar(null, item))
                            canvas.DrawCircle(x, y, (AMINORADIUS * percentDiff), PolarAminoAcid);
                        else
                            canvas.DrawCircle(x, y, (AMINORADIUS * percentDiff), NonPolarAminoAcid);
                    }
                    else
                    {
                        canvas.DrawCircle(x, y, (AMINORADIUS * percentDiff), InvalidAminoAcid);
                    }
                    angle += 100;
                    if (angle > 360)
                        angle -= 360;
                    lastChar = item;
                }
            }
            if (char.IsLetter(lastChar))
            {
                if (polarity)
                    canvas.DrawText(lastChar.ToString().ToUpper() + incr, x - (6 * percentDiff), y + (4 * percentDiff), PolarLetters);
                else
                    canvas.DrawText(lastChar.ToString().ToUpper() + incr, x - (6 * percentDiff), y + (4 * percentDiff), NonPolarLetters);
            }
        }
        public void HelicalWheelBuilderThreeLetter(string aminoAcids, SKCanvas canvas)
        {
            var aminoClass = new AminoAcids();
            var listAminos = aminoAcids.Split(',').ToList();
            float xval = 0; float yval = 0;
            float xModifier = 0; float yModifier = 0;
            float modIncrementer = 0;
            float scaleModifier = 19;
            HelicalStructure.Clear();
            int angle = 270;
            bool polarity = false;
            string lastAmino ="";
            int incr = 1;
            var percentDiff = App.ScreenHeight > App.ScreenWidth ? ((App.ScreenHeight / App.ScreenWidth) / ScaleFactor) + WheelEnlarger : ((App.ScreenWidth / App.ScreenHeight) / ScaleFactor) + WheelEnlarger;
            foreach (var item in listAminos)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    if (!string.IsNullOrWhiteSpace(lastAmino))
                    {
                        if(incr < 18)
                            canvas.DrawLine(xval, yval, (float)((RADIUS * percentDiff) * Math.Cos(angle * Math.PI / 180F)), (float)((RADIUS * percentDiff) * Math.Sin(angle * Math.PI / 180F)), Line);
                        if (polarity)
                            canvas.DrawText(lastAmino.Substring(0, 1).ToString().ToUpper() + lastAmino.Substring(1,2) + incr, xval - (7 * percentDiff), yval + (4 * percentDiff), PolarLetters);
                        else
                            canvas.DrawText(lastAmino.Substring(0, 1).ToString().ToUpper() + lastAmino.Substring(1,2) + incr, xval - (7 * percentDiff), yval + (4 * percentDiff), NonPolarLetters);
                        incr += 1;
                    }
                    xval = (float)((RADIUS * percentDiff) * Math.Cos(angle * Math.PI / 180F));
                    yval = (float)((RADIUS * percentDiff) * Math.Sin(angle * Math.PI / 180F));
                    HelicalStructure.Add(new KeyValuePair<string, Point>(item.Substring(0, 1).ToString().ToUpper() + item.Substring(1, 2) + incr, new Point(xval, yval)));
                    if (incr % scaleModifier == 0)
                    {
                        modIncrementer += 1;
                        scaleModifier = scaleModifier + 18;
                    }
                    xModifier = xval > 0 ? 13f * modIncrementer * percentDiff : -13f * modIncrementer * percentDiff;
                    yModifier = yval > 0 ? 13f * modIncrementer * percentDiff : -13f * modIncrementer * percentDiff;
                    if (Math.Abs(xval) < 5 && modIncrementer > 0)
                    {
                        yModifier = yModifier > 0 ? (yModifier - (.5f * percentDiff)) + (6 * modIncrementer * percentDiff) : (yModifier + (.5f * percentDiff)) - (6 * modIncrementer * percentDiff);
                        xModifier = 0;
                    }
                    xval += xModifier;
                    yval += yModifier;
                    if (aminoClass.IsAminoAcid(item))
                    {
                        if (polarity = aminoClass.IsPolar(item))
                            canvas.DrawCircle(xval, yval, (AMINORADIUS * percentDiff), PolarAminoAcid);
                        else
                            canvas.DrawCircle(xval, yval, (AMINORADIUS * percentDiff), NonPolarAminoAcid);
                    }
                    else
                    {
                        canvas.DrawCircle(xval, yval, (AMINORADIUS * percentDiff), InvalidAminoAcid);
                    }
                    angle += 100;
                    if (angle > 360)
                        angle -= 360;
                    lastAmino = item;
                }
            }
            if (!string.IsNullOrWhiteSpace(lastAmino))
            {
                if (polarity)
                    canvas.DrawText(lastAmino.Substring(0, 1).ToString().ToUpper() + lastAmino.Substring(1, 2) + incr, xval - (7 * percentDiff), yval + (4 * percentDiff), PolarLetters);
                else
                    canvas.DrawText(lastAmino.Substring(0, 1).ToString().ToUpper() + lastAmino.Substring(1, 2) + incr, xval - (7 * percentDiff), yval + (4 * percentDiff), NonPolarLetters);
            }
        }
    }
}
