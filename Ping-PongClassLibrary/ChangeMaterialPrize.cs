using System;

namespace Ping_PongClassLibrary
{
    public class ChangeMaterialPrize : Prize
    {
        /// <summary>
        /// Инициализирует новый экземпляр приза ChangeMaterialPrize, который изменяет материал ракетки.
        /// </summary>
        public ChangeMaterialPrize(double x, double y, double width, double height, int textureId, double spawnTime)
            : base(x, y, width, height, textureId, spawnTime) { }

        /// <summary>
        /// Применяет эффект приза к ракетке, оборачивая её в декоратор <see cref="MaterialChangeDecorator"/>, который изменяет материал ракетки.
        /// </summary>
        public override IPaddle Apply(IPaddle paddle)
        {
            return new MaterialChangeDecorator(paddle);
        }
    }
}