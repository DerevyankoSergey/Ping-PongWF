using System;

namespace Ping_PongClassLibrary
{
    public class IncreaseWidthPrize : Prize
    {
        /// <summary>
        /// Инициализирует новый экземпляр приза IncreaseWidthPrize, который увеличивает ширину ракетки.
        /// </summary>
        public IncreaseWidthPrize(double x, double y, double width, double height, int textureId, double spawnTime)
            : base(x, y, width, height, textureId, spawnTime) { }

        /// <summary>
        /// Применяет эффект приза к ракетке, оборачивая её в декоратор WidthIncreaseDecorator, который увеличивает ширину ракетки.
        /// </summary>
        public override IPaddle Apply(IPaddle paddle)
        {
            return new WidthIncreaseDecorator(paddle);
        }
    }
}