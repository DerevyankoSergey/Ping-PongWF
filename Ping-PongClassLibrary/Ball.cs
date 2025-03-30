using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public class Ball
    {
        // Свойства мяча
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Vx { get; private set; }
        public double Vy { get; private set; }
        public double Radius { get; private set; }
        public double Mass { get; private set; }
        private bool hasCollidedWithPaddle;

        private readonly Random random = new Random();

        public Ball(double x, double y,double radius, double mass)
        {
            X = x;
            Y = y;
            Radius = radius;
            Mass = mass;
            Vx = 0;
            Vy = 0;
            hasCollidedWithPaddle = false;
        }

        // Обновление позиции мяча
        public void Update(double deltaTime, int screenWidth, int screenHeight, int tableLeft, int tableRight, int tableTop, int tableBottom, bool isPlayer1Serving)
        {
            // Если мяч не движется (скорость нулевая)
            if (Vx == 0 && Vy == 0)
                return;

            X += Vx * deltaTime;
            Y += Vy * deltaTime;

            // Проверка выхода за верхнюю сторону стола
            if (Y - Radius < tableTop)
            {
                ResetForServe(screenWidth, screenHeight, tableLeft, tableRight, tableTop, tableBottom, isPlayer1Serving);
                return;
            }
            // Проверка выхода за нижнюю сторону стола
            else if (Y + Radius > tableBottom)
            {
                ResetForServe(screenWidth, screenHeight, tableLeft, tableRight, tableTop, tableBottom, isPlayer1Serving);
                return;
            }

            // Проверка выхода за левую боковую сторону стола
            if (X -  Radius < tableLeft)
            {
                ResetForServe(screenWidth, screenHeight, tableLeft, tableRight, tableTop, tableBottom, !isPlayer1Serving);
                return;
            }
            // Проверка выхода за правую боковую сторону стола
            else if (X +  Radius > tableRight)
            {
                ResetForServe(screenWidth, screenHeight, tableLeft, tableRight, tableTop, tableBottom, !isPlayer1Serving);
                return;
            }
        }

        // Сброс мяча для подачи
        public void ResetForServe(int screenWidth, int screenHeight, int tableLeft, int tableRight, int tableTop, int tableBottom, bool isPlayer1Serving)
        {
            double centerY = (tableTop + tableBottom) / 2.0;

            if (isPlayer1Serving)
            {
                X = tableLeft + 20;
                Y = centerY;
            }
            else
            {
                X = tableRight - 20;
                Y = centerY;
            }

            Vx = 0;
            Vy = 0;
            hasCollidedWithPaddle = false;
        }

        //Подача
        public void Serve(double initialSpeed, bool isLeft)
        {
            Vx = initialSpeed * (isLeft ? 1 : -1); 
            Vy = (random.NextDouble() - 0.5) * initialSpeed; 
            hasCollidedWithPaddle = false;
        }

        //Столкновение мяча с ракеткой
        public void CollideWithPaddle(IPaddle paddle, bool isLeftPaddle)
        {
            //Проверка столкновения
            bool hitX = (isLeftPaddle && X - Radius <= paddle.X + paddle.Width && X - Radius >= paddle.X) ||
                (!isLeftPaddle && X + Radius >= paddle.X && X + Radius <= paddle.X + paddle.Width);

            bool hitY = Y + Radius >= paddle.Y && Y - Radius <= paddle.Y + paddle.Height;

            if (hitX && hitY)
            {
                hasCollidedWithPaddle = true;

                double speed = Math.Sqrt(Vx * Vx + Vy * Vy);
                double incomingAngle = Math.Atan2(Vy, Vx);

                //Расчет точки удара
                double hitYPos = (paddle.Y + paddle.Height / 2) - Y;
                double hitYNorm = hitYPos/(paddle.Height/2);

                double maxAngel = Math.PI / 2;
                double angelOffset = hitYNorm * maxAngel;

                double paddleEffect = paddle.Vy * 0.1;

                //Угол отскока
                double newAngle;
                if (isLeftPaddle)
                    newAngle = Math.PI - incomingAngle + angelOffset;
                else
                    newAngle = -incomingAngle + angelOffset;

                newAngle = (newAngle + 2 * Math.PI) % (2*Math.PI);
                if (newAngle > Math.PI) newAngle -= 2 * Math.PI;


                //Новая скорость
                double bounce = paddle.Elasticity ?? 1.0;
                double newSpeed = speed * bounce;

                //Применение новых скоростей
                Vx = newSpeed * Math.Cos(newAngle);
                Vy = newSpeed * Math.Sin(newAngle) + paddleEffect;

                if (isLeftPaddle)
                    X = paddle.X + paddle.Width + Radius;
                else
                    X = paddle.X - Radius;

            }

        }
    }
}
