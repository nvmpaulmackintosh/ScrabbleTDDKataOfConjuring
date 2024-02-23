using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrabble.Lib
{
    public class BoardScoreCalculator
    {
        public static int ScoreWord(IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
        {
            int score = 0;
            int wordFactor = 1;

            var extendedScore = 0;
            extendedScore += CalculateHorizontalExtendedScore(laidTiles, boardSquares, true);
            extendedScore += CalculateHorizontalExtendedScore(laidTiles, boardSquares, false);

            foreach ((Square square, Tile tile) in laidTiles)
            {
                score += tile.Value * square.Type.LetterFactor;

                wordFactor = Math.Max(wordFactor, square.Type.WordFactor);
            }

            return (score * wordFactor) + extendedScore;
        }

        private static int CalculateHorizontalExtendedScore(IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares, bool isPrefix)
        {
            int score = 0;
            int directionFactor = 1;
            if (isPrefix)
            {
                directionFactor *= -1;
            }
            var rightmostSquare = laidTiles.Select(x => x.Square).OrderBy(x => x.Point.X * directionFactor).Last();

            while (boardSquares.Any(x => x.Point.X == rightmostSquare.Point.X + directionFactor))
            {
                var square = boardSquares
                    .Where(x => x.State is Occupied)
                    .Where(x => x.Point.Y == rightmostSquare.Point.Y)
                    .SingleOrDefault(x => x.Point.X == rightmostSquare.Point.X + directionFactor);

                if (square == null) break;
                var occupied = square.State as Occupied;
                rightmostSquare = square;

                score += occupied.Tile.Value;
            }

            return score;
        }
    }
}