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
            extendedScore += CalculateExtendedScore(laidTiles, boardSquares, true, true);
            extendedScore += CalculateExtendedScore(laidTiles, boardSquares, false, true);
            extendedScore += CalculateExtendedScore(laidTiles, boardSquares, true, false);
            extendedScore += CalculateExtendedScore(laidTiles, boardSquares, false, false);

            foreach ((Square square, Tile tile) in laidTiles)
            {
                score += tile.Value * square.Type.LetterFactor;

                wordFactor = Math.Max(wordFactor, square.Type.WordFactor);
            }

            return (score * wordFactor) + extendedScore;
        }

        private static int CalculateExtendedScore(IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares, bool isPrefix, bool isHorizontal)
        {
            int score = 0;

            int directionFactor = 1;
            if (isPrefix)
            {
                directionFactor *= -1;
            }

            Func<Square, int> getHorizontalCoordinate = (Square square) => square.Point.X;
            Func<Square, int> getVerticalCoordinate = (Square square) => square.Point.Y;

            var getInlineCoordinate = isHorizontal ? getHorizontalCoordinate : getVerticalCoordinate;
            var getPerpendicularCoordinate = isHorizontal ? getVerticalCoordinate : getHorizontalCoordinate;

            var outermostSquare = laidTiles.Select(laidTile => laidTile.Square)
                .OrderBy(square => getInlineCoordinate(square) * directionFactor)
                .Last();

            while (boardSquares.Any(square => getInlineCoordinate(square) == getInlineCoordinate(outermostSquare) + directionFactor))
            {
                var square = boardSquares
                    .Where(x => x.State is Occupied)
                    .Where(x => getPerpendicularCoordinate(x) == getPerpendicularCoordinate(outermostSquare))
                    .SingleOrDefault(x => getInlineCoordinate(x) == getInlineCoordinate(outermostSquare) + directionFactor);

                if (square == null) break;
                var occupied = square.State as Occupied;
                outermostSquare = square;

                score += occupied.Tile.Value;
            }

            return score;
        }
    }
}