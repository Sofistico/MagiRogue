//using Arquimedes.Enumerators;
//using MagusEngine.Core.WorldStuff;
//using System.Collections.Generic;

//namespace MagusEngine.ECS.Components.TilesComponents
//{
//    // redo from scratch
//    public class RiverTile
//    {
//        public List<River> Rivers { get; set; } = new();

//        public int RiverSize { get; set; }

//        public int GetRiverNeighborCount(River river)
//        {
//            try
//            {
//                int count = 0;
//                if (Left.Rivers.Count > 0 && Left.Rivers.Contains(river))
//                    count++;
//                if (Right.Rivers.Count > 0 && Right.Rivers.Contains(river))
//                    count++;
//                if (Top.Rivers.Count > 0 && Top.Rivers.Contains(river))
//                    count++;
//                if (Bottom.Rivers.Count > 0 && Bottom.Rivers.Contains(river))
//                    count++;
//                return count;
//            }
//            catch (Exception)
//            {
//                throw new Exception("An error occured in the river count method!");
//            }
//        }

//        public void SetRiverPath(River river)
//        {
//            try
//            {
//                if (!Collidable)
//                    return;

//                if (!Rivers.Contains(river))
//                {
//                    Rivers.Add(river);
//                }
//            }
//            catch (Exception)
//            {
//                throw new Exception("Recursive river generation failed!");
//            }
//        }

//        private void SetRiverTile(River river)
//        {
//            SetRiverPath(river);
//            HeightType = HeightType.River;
//            HeightValue = 0;
//            Collidable = false;
//        }

//        public void DigRiver(River river, int size)
//        {
//            SetRiverTile(river);
//            RiverSize = size;

//            if (size == 1)
//            {
//                Bottom.SetRiverTile(river);
//                Right.SetRiverTile(river);
//                Bottom.Right.SetRiverTile(river);
//            }

//            if (size == 2)
//            {
//                Bottom.SetRiverTile(river);
//                Right.SetRiverTile(river);
//                Bottom.Right.SetRiverTile(river);
//                Top.SetRiverTile(river);
//                Top.Left.SetRiverTile(river);
//                Top.Right.SetRiverTile(river);
//                Left.SetRiverTile(river);
//                Left.Bottom.SetRiverTile(river);
//            }

//            if (size == 3)
//            {
//                Bottom.SetRiverTile(river);
//                Right.SetRiverTile(river);
//                Bottom.Right.SetRiverTile(river);
//                Top.SetRiverTile(river);
//                Top.Left.SetRiverTile(river);
//                Top.Right.SetRiverTile(river);
//                Left.SetRiverTile(river);
//                Left.Bottom.SetRiverTile(river);
//                Right.Right.SetRiverTile(river);
//                Right.Right.Bottom.SetRiverTile(river);
//                Bottom.Bottom.SetRiverTile(river);
//                Bottom.Bottom.Right.SetRiverTile(river);
//            }

//            if (size == 4)
//            {
//                Bottom.SetRiverTile(river);
//                Right.SetRiverTile(river);
//                Bottom.Right.SetRiverTile(river);
//                Top.SetRiverTile(river);
//                Top.Right.SetRiverTile(river);
//                Left.SetRiverTile(river);
//                Left.Bottom.SetRiverTile(river);
//                Right.Right.SetRiverTile(river);
//                Right.Right.Bottom.SetRiverTile(river);
//                Bottom.Bottom.SetRiverTile(river);
//                Bottom.Bottom.Right.SetRiverTile(river);
//                Left.Bottom.Bottom.SetRiverTile(river);
//                Left.Left.Bottom.SetRiverTile(river);
//                Left.Left.SetRiverTile(river);
//                Left.Left.Top.SetRiverTile(river);
//                Left.Top.SetRiverTile(river);
//                Left.Top.Top.SetRiverTile(river);
//                Top.Top.SetRiverTile(river);
//                Top.Top.Right.SetRiverTile(river);
//                Top.Right.Right.SetRiverTile(river);
//            }
//        }
//    }
//}
