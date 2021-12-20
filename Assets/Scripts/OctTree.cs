namespace octTree
{
    public class OctTreeInt
    {
        private readonly OctTreeNodeInt root;

        public OctTreeInt(int[] baseData, int x, int y, int z)
        {
            root = OctTreeNodeInt.BuildOctTree(x, y, z, baseData);
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Collections.Generic.Queue<OctTreeNodeInt> levelOrderedQueue = new System.Collections.Generic.Queue<OctTreeNodeInt>();

            // add the root to the queue
            levelOrderedQueue.Enqueue(root);

            // loop though all of the nodes on the tree
            while(levelOrderedQueue.Count > 0)
            {
                // get the current object to view the toString of
                OctTreeNodeInt current = levelOrderedQueue.Dequeue();

                // collect all of the children
                for(int index = 0; index < current.Children.Length; index++)
                {
                    if (current.Children[index] != null)
                        levelOrderedQueue.Enqueue(current.Children[index]);
                }

                // append the currents to string before moving on
                sb.Append(current.ToString());
            }

            // to string 
            return sb.ToString();
        }

        class OctTreeNodeInt
        {
            public int Count;

            public int Level;

            public int MaxLevel;

            public int Average = 0;

            public int Max = int.MinValue;

            public int Min = int.MaxValue;

            public OctTreeNodeInt[] Children = new OctTreeNodeInt[0];

            public int minX, minY, minZ;

            public int maxX, maxY, maxZ;

            public static OctTreeNodeInt BuildOctTree(int x, int y, int z, int[] data)
            {
                // create a complet new tree by recucivly creating a tree node
                return new OctTreeNodeInt(0, 0, 0, x, y, z, data, z, y, z, 0);
            }

            private OctTreeNodeInt(int minX, int minY, int minZ, int maxX, int maxY, int maxZ, int[] data, int originalX, int originalY, int OriginalZ, int level)
            {
                // save the current cords
                this.minX = minX;
                this.maxX = maxX;
                this.minY = minY;
                this.maxY = maxY;
                this.minZ = minZ;
                this.maxZ = maxZ;

                // counter
                Count = (maxX - minX) * (maxY - minY) * (maxZ - minZ);
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
                    Children = new OctTreeNodeInt[8];

                    int changeX = UnityEngine.Mathf.CeilToInt((maxX - minX) / 2f);
                    int changeY = UnityEngine.Mathf.CeilToInt((maxY - minY) / 2f);
                    int changeZ = UnityEngine.Mathf.CeilToInt((maxZ - minZ) / 2f);

                    if (changeX < 0) changeX = 0;
                    if (changeY < 0) changeY = 0;
                    if (changeZ < 0) changeZ = 0;

                    // create all of the children objects
                    if (calcSize(minX, minY, minZ, maxX - changeX, maxY - changeY, maxZ - changeZ) > 0)
                        Children[0] = new OctTreeNodeInt(minX, minY, minZ, maxX - changeX, maxY - changeY, maxZ - changeZ, data, originalX, originalY, OriginalZ, level + 1);
                    if (calcSize(minX, maxY - changeY, minZ, maxX - changeX, maxY, maxZ - changeZ)> 0)
                        Children[1] = new OctTreeNodeInt(minX, maxY - changeY, minZ, maxX - changeX, maxY, maxZ - changeZ, data, originalX, originalY, OriginalZ, level + 1);
                    if (calcSize(maxX - changeX, minY, minZ, maxX, maxY - changeY, maxZ - changeZ) > 0)
                        Children[2] = new OctTreeNodeInt(maxX - changeX, minY, minZ, maxX, maxY - changeY, maxZ - changeZ, data, originalX, originalY, OriginalZ, level + 1);
                    if (calcSize(maxX - changeX, maxY - changeY, minZ, maxX, maxY, maxZ - changeZ) > 0)
                        Children[3] = new OctTreeNodeInt(maxX - changeX, maxY - changeY, minZ, maxX, maxY, maxZ - changeZ, data, originalX, originalY, OriginalZ, level + 1);

                    if (calcSize(minX, minY, maxZ - changeZ, maxX - changeX, maxY - changeY, maxZ) > 0)
                        Children[4] = new OctTreeNodeInt(minX, minY, maxZ - changeZ, maxX - changeX, maxY - changeY, maxZ, data, originalX, originalY, OriginalZ, level + 1);
                    if (calcSize(minX, maxY - changeY, maxZ - changeZ, maxX - changeX, maxY, maxZ) > 0)
                        Children[5] = new OctTreeNodeInt(minX, maxY - changeY, maxZ - changeZ, maxX - changeX, maxY, maxZ, data, originalX, originalY, OriginalZ, level + 1);
                    if (calcSize(maxX - changeX, minY, maxZ - changeZ, maxX, maxY - changeY, maxZ) > 0)
                        Children[6] = new OctTreeNodeInt(maxX - changeX, minY, maxZ - changeZ, maxX, maxY - changeY, maxZ, data, originalX, originalY, OriginalZ, level + 1);
                    if (calcSize(maxX - changeX, maxY - changeY, maxZ - changeZ, maxX, maxY, maxZ) > 0)
                        Children[7] = new OctTreeNodeInt(maxX - changeX, maxY - changeY, maxZ - changeZ, maxX, maxY, maxZ, data, originalX, originalY, OriginalZ, level + 1);

                    InializeNode();
                }
            }

            private static int calcSize(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
            {
                return (maxX - minX) * (maxY - minY) * (maxZ - minZ);
            }

            private void InitalizeLeafNode(int x, int y, int z, int[] data, int originalX, int originalY, int OriginalZ, int level)
            {
                // get the data point
                int index = this.Get(x, y, z, originalX, originalY, OriginalZ);

                // set all the values to the data entries value
                this.Max = data[index];
                this.Min = data[index];
                this.Average = data[index];

                // set the tree data
                this.MaxLevel = this.Level;
            }

            private void InializeNode()
            {
                for (int index = 0; index < Children.Length; index++)
                {
                    if (Children[index] != null)
                    {
                        // set the max value found
                        if (Children[index].Max > this.Max)
                        {
                            this.Max = Children[index].Max;
                        }

                        // set the min value found
                        if (Children[index].Min < this.Min)
                        {
                            this.Min = Children[index].Min;
                        }

                        // get values for the averate
                        this.Average += Children[index].Average;

                        // set the max value found
                        if (Children[index].MaxLevel > this.MaxLevel)
                        {
                            this.MaxLevel = Children[index].MaxLevel;
                        }
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
                sb.Append(this.Count);
                sb.AppendLine("}");
                return sb.ToString();
            }
        }
    }

    
}

