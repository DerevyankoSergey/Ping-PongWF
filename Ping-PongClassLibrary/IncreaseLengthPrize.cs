using System;

namespace Ping_PongClassLibrary
{
    public class IncreaseLengthPrize : Prize
    {
        /// <summary>
        /// Инициализирует новый экземпляр приза IncreaseLengthPrize, который увеличивает длину ракетки.
        /// </summary>
        public IncreaseLengthPrize(double x, double y, double width, double height, int textureId, double spawnTime)
            : base(x, y, width, height, textureId, spawnTime) { }

        /// <summary>
        /// Применяет эффект приза к ракетке, оборачивая её в декоратор LengthIncreaseDecorator, который увеличивает длину ракетки.
        /// </summary>
        public override IPaddle Apply(IPaddle paddle)
        {
            return new LengthIncreaseDecorator(paddle);
        }
    }
}