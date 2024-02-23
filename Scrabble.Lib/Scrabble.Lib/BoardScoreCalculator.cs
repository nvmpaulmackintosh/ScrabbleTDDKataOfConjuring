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
            extendedScore += CalculateHorizontalSuffixScore(laidTiles, boardSquares);

            foreach ((Square square, Tile tile) in laidTiles)
            {
                score += tile.Value * square.Type.LetterFactor;

                wordFactor = Math.Max(wordFactor, square.Type.WordFactor);
            }

            return (score * wordFactor) + extendedScore;
        }

        private static int CalculateHorizontalSuffixScore(IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
        {
            int score = 0;
            var leftmostSquare = laidTiles.Select(x => x.Square).OrderBy(x => x.Point.X).First();

            while (boardSquares.Any(x => x.Point.X == (leftmostSquare.Point.X-1)))
            {
                var square = boardSquares
                    .Where(x => x.State is Occupied)
                    .Where(x => x.Point.Y == leftmostSquare.Point.Y)
                    .SingleOrDefault(x => x.Point.X == leftmostSquare.Point.X - 1);

                if (square == null) break;
                var occupied = square.State as Occupied;
                leftmostSquare = square;

                score += occupied.Tile.Value;
            }

            return score;
        }
    }
}