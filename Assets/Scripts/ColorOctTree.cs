using UnityEngine;

namespace octTree
{
    public class OctTreeColor
    {
        readonly OctTreeNodeColor root;

        public OctTreeColor(Color[] baseData, int x, int y, int z)
        {
            root = OctTreeNodeColor.BuildOctTree(z, y, z, baseData);
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Collections.Generic.Queue<OctTreeNodeColor> levelOrderedQueue = new System.Collections.Generic.Queue<OctTreeNodeColor>();

            // add the root to the queue
            levelOrderedQueue.Enqueue(root);

            // loop though all of the nodes on the tree
            while (levelOrderedQueue.Count > 0)
            {
                // get the current object to view the toString of
                OctTreeNodeColor current = levelOrderedQueue.Dequeue();

                // collect all of the children
                for (int index = 0; index < current.Children.Length; index++)
                {
                    levelOrderedQueue.Enqueue(root);
                }

                // append the currents to string before moving on
                sb.Append(current.ToString());
            }

            // to string 
            return sb.ToString();
        }
    }

    class OctTreeNodeColor
    {
        public int Count;

        public int Level;

        public int MaxLevel;

        public Color Average;

        public Color Max = Color.clear;

        public Color Min = Color.white;

        public OctTreeNodeColor[] Children = null;

        public int minX, minY, minZ;

        public int maxX, maxY, maxZ;

        public static OctTreeNodeColor BuildOctTree(int x, int y, int z, Color[] data)
        {
            // create a complet new tree by recucivly creating a tree node
            return new OctTreeNodeColor(0, 0, 0, x, y, z, data, z, y, z, 0);
        }

        private OctTreeNodeColor(int minX, int minY, int minZ, int maxX, int maxY, int maxZ, Color[] data, int originalX, int originalY, int OriginalZ, int level)
        {
            // save the current cords
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            this.minZ = minZ;
            this.maxZ = maxZ;

            Count = (maxX - minX) + (maxY - minY) + (maxZ - minZ);
            this.Level = level;

            // depending on the size of the argument will change what this will do
            if (Count <= 1)
            {
                // create a leaf node for this object
                InitalizeLeafNode(minX, minY, minZ, data, originalX, originalY, OriginalZ, level + 1);
            }
            else
            {
                // create leaf nodes for all instances
                Children = new OctTreeNodeColor[8];

                int changeX = UnityEngine.Mathf.CeilToInt((maxX - minX) / 2f);
                int changeY = UnityEngine.Mathf.CeilToInt((maxY - minY) / 2f);
                int changeZ = UnityEngine.Mathf.CeilToInt((maxZ - minZ) / 2f);

                if (changeX < 0) changeX = 0;
                if (changeY < 0) changeY = 0;
                if (changeZ < 0) changeZ = 0;

                // create all of the children objects
                if (CalcSize(minX, minY, minZ, maxX - changeX, maxY - changeY, maxZ - changeZ) > 0)
                    Children[0] = new OctTreeNodeColor(minX, minY, minZ, maxX - changeX, maxY - changeY, maxZ - changeZ, data, originalX, originalY, OriginalZ, level + 1);
                if (CalcSize(minX, maxY - changeY, minZ, maxX - changeX, maxY, maxZ - changeZ) > 0)
                    Children[1] = new OctTreeNodeColor(minX, maxY - changeY, minZ, maxX - changeX, maxY, maxZ - changeZ, data, originalX, originalY, OriginalZ, level + 1);
                if (CalcSize(maxX - changeX, minY, minZ, maxX, maxY - changeY, maxZ - changeZ) > 0)
                    Children[2] = new OctTreeNodeColor(maxX - changeX, minY, minZ, maxX, maxY - changeY, maxZ - changeZ, data, originalX, originalY, OriginalZ, level + 1);
                if (CalcSize(maxX - changeX, maxY - changeY, minZ, maxX, maxY, maxZ - changeZ) > 0)
                    Children[3] = new OctTreeNodeColor(maxX - changeX, maxY - changeY, minZ, maxX, maxY, maxZ - changeZ, data, originalX, originalY, OriginalZ, level + 1);

                if (CalcSize(minX, minY, maxZ - changeZ, maxX - changeX, maxY - changeY, maxZ) > 0)
                    Children[4] = new OctTreeNodeColor(minX, minY, maxZ - changeZ, maxX - changeX, maxY - changeY, maxZ, data, originalX, originalY, OriginalZ, level + 1);
                if (CalcSize(minX, maxY - changeY, maxZ - changeZ, maxX - changeX, maxY, maxZ) > 0)
                    Children[5] = new OctTreeNodeColor(minX, maxY - changeY, maxZ - changeZ, maxX - changeX, maxY, maxZ, data, originalX, originalY, OriginalZ, level + 1);
                if (CalcSize(maxX - changeX, minY, maxZ - changeZ, maxX, maxY - changeY, maxZ) > 0)
                    Children[6] = new OctTreeNodeColor(maxX - changeX, minY, maxZ - changeZ, maxX, maxY - changeY, maxZ, data, originalX, originalY, OriginalZ, level + 1);
                if (CalcSize(maxX - changeX, maxY - changeY, maxZ - changeZ, maxX, maxY, maxZ) > 0)
                    Children[7] = new OctTreeNodeColor(maxX - changeX, maxY - changeY, maxZ - changeZ, maxX, maxY, maxZ, data, originalX, originalY, OriginalZ, level + 1);

                InializeNode();
            }
        }

        private static int CalcSize(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            return (maxX - minX) * (maxY - minY) * (maxZ - minZ);
        }

        private void InitalizeLeafNode(int x, int y, int z, Color[] data, int originalX, int originalY, int OriginalZ, int level)
        {
            // get the data point
            int index = this.Get(x, y, z, originalX, originalY, OriginalZ);

            // set all the values to the data entries value
            this.Max = data[index];
            this.Min = data[index];
            this.Average = data[index];

            // set the tree data
            this.Level = level;
            this.MaxLevel = this.Level;
        }

        private void InializeNode()
        {
            for (int index = 0; index < Children.Length; index++)
            {
                // set the max value found
                if (Children[index].Max.r > this.Max.r)
                {
                    this.Max.r = Children[index].Max.r;
                }
                if (Children[index].Max.g > this.Max.g)
                {
                    this.Max.g = Children[index].Max.g;
                }
                if (Children[index].Max.b > this.Max.b)
                {
                    this.Max.b = Children[index].Max.b;
                }
                if (Children[index].Max.a > this.Max.a)
                {
                    this.Max.a = Children[index].Max.a;
                }

                // set the min value found
                if (Children[index].Min.r < this.Min.r)
                {
                    this.Min.r = Children[index].Min.r;
                }
                if (Children[index].Min.g < this.Min.g)
                {
                    this.Min.g = Children[index].Min.g;
                }
                if (Children[index].Min.b < this.Min.b)
                {
                    this.Min.b = Children[index].Min.b;
                }
                if (Children[index].Min.a < this.Min.a)
                {
                    this.Min.a = Children[index].Min.a;
                }

                // get values for the average
                this.Average += Children[index].Average;

                // set the max value found
                if (Children[index].MaxLevel > this.MaxLevel)
                {
                    this.MaxLevel = Children[index].MaxLevel;
                }
            }

            // finalize the average
            this.Average /= this.Children.Length;
        }

        public int Get(int x, int y, int z, int originalX, int originalY, int OriginalZ)
        {
            return (z * (originalY * originalX)) + (y * originalX) + x;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{");
            sb.Append("level: ");
            sb.Append(Level);
            sb.Append(",Min: ");
            sb.Append(minX);
            sb.Append(",");
            sb.Append(minY);
            sb.Append(",");
            sb.Append(minZ);
            sb.Append(",");
            sb.Append("Max: ");
            sb.Append(maxX);
            sb.Append(",");
            sb.Append(maxY);
            sb.Append(",");
            sb.Append(maxZ);
            sb.Append(",");
            sb.Append("Count: ");
            sb.Append(Count);
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}

