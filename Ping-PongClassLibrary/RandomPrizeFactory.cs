using System;

namespace Ping_PongClassLibrary
{
    /// <summary>
    /// Фабрика для создания случайных призов с заданными параметрами.
    /// </summary>
    public class RandomPrizeFactory : PrizeFactory
    {
        private readonly double width = 40;
        private readonly double height = 40;

        /// <summary>
        /// Создаёт приз на основе указанного идентификатора текстуры.
        /// </summary>
        public override IPrize CreatePrize(double x, double y, int textureId, double spawnTime)
        {
            switch (textureId)
            {
                case 4: 
                    return new IncreaseWidthPrize(x, y, width, height, textureId, spawnTime);
                case 5:
                    return new IncreaseLengthPrize(x, y, width, height, textureId, spawnTime);
                case 6: 
                    return new ChangeMaterialPrize(x, y, width, height, textureId, spawnTime);
                default:
                    throw new InvalidOperationException($"Неизвестный textureId: {textureId}");
            }
        }
    }
}