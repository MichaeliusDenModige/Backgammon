﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backgammon.Screen;
using Microsoft.Xna.Framework;
using ModelDLL;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace Backgammon.Object
{
    public class Point
    {
        protected int Number { get; private set; }
        protected Vector2 Position { get; private set; }
        protected List<Checker> Checkers { get; private set; }
        private Image Image = new Image() { Path = "Images/Glow", Effects = "FadeEffect", IsActive = false};
        internal Vector2 ReceivingPosition { get; private set; }

        // Modifies Y distance (if < 5) between checkers
        private readonly static float checkerDistance = 33; // other values could be [25,50]

        // Modifies Y distance for glow effect.
        private readonly static float YModifier = 90;

        // Do not modify.
        private readonly static float MiddleY = 720 / 2;

        internal int GetAmount()
        {
            return Checkers.Count;
        }

        internal Rectangle GetBounds()
        {
            return Image.GetBounds();
        }

        internal Point(Vector2 Position, List<Checker> Checkers)
        {
            this.Position = Position;
            this.Checkers = Checkers;
            ArrangeCheckers();

            Image.Position = Position;
            if (Position.Y > MiddleY)
                Image.Position += new Vector2(0, -YModifier);
            else
            {
                Image.Position += new Vector2(0, YModifier);
                Image.SpriteEffect = SpriteEffects.FlipVertically;
            }
            Image.LoadContent();
            Image.FadeEffect.FadeSpeed = 1f;
        }

        protected void AddChecker(Checker checker)
        {
            Checkers.Add(checker);
        }

        protected void RemoveChecker(Checker checker)
        {
            Checkers.Remove(checker);
            ArrangeCheckers();
        }

        internal void SendToPoint(Point p)
        {
            Glow(false);
            Checker c = GetTopChecker();
            c.MoveToPoint(p);
            RemoveChecker(c);
            p.AddChecker(c);
        }

        internal bool IsEmpty()
        {
            return (Checkers.Count == 0);
        }

        internal Checker GetTopChecker()
        {
            if (IsEmpty())
                return null;
            else
                return Checkers.ElementAt(Checkers.Count - 1);
        }

        internal void Glow(bool b)
        {
            if (Image.IsActive && b)
                return; // If glowing is already activated, do not disturb current animation.
            Image.IsActive = Image.FadeEffect.Increase = b;
            Image.Alpha = 0.0f;
        }

        internal void ArrangeCheckers()
        {
            float dist = checkerDistance;
            //if (Checkers.Count > 5)
            //    dist = checkerDistance * (1 / Checkers.Count);

            for (int i = 0; i < Checkers.Count; i++)
                if (Position.Y > 360) // Is this Point at bottom?
                    Checkers[i].SetPosition(Position.X, Position.Y - i * checkerDistance);
                else
                    Checkers[i].SetPosition(Position.X, Position.Y + i * checkerDistance);

            if (Position.Y > 360)
                ReceivingPosition = new Vector2(Position.X, Position.Y - Checkers.Count * checkerDistance);
            else
                ReceivingPosition = new Vector2(Position.X, Position.Y + Checkers.Count * checkerDistance);
        }

        internal void Update(GameTime gameTime)
        {
            Image.Update(gameTime);
            foreach (Checker c in Checkers)
                c.Update(gameTime);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
            foreach (Checker c in Checkers)
                c.Draw(spriteBatch);
        }

    }
}
