/* Name: QuadTree Data Structure (Assignment #3)
 * Author: Brianna Drew
 * Date Created: March 19th, 2020
 * Last Modified: April 6th, 2020
 * References: - https://en.wikipedia.org/wiki/Quadtree
 *             - http://people.scs.carleton.ca/~maheshwa/courses/5703COMP/16Fall/quadtrees-paper.pdf
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadTree
{
    public static class MyGlobals
    {
        public static bool EXIT, ERROR;
        public static int Union_Count;
    }

    public class Node<T>
    {
        public int Colour { get; set; }     // integer to represent colour of node
        public enum Colours { G, W, B };     // enumerated types to represent possible node colours
        public Node<T> NW { get; set; }     // reference to north-west quadrant
        public Node<T> NE { get; set; }     // reference to north-east quadrant
        public Node<T> SE { get; set; }     // reference to south-east quadrant
        public Node<T> SW { get; set; }     // reference to south-west quadrant

        public Node(int colour)
        {
            Colour = colour;
            NW = NE = SE = SW = null;
        }
    }

    class QuadTree<T>
    {
        public Node<T> Root;
        public int Size;

        public QuadTree()
        {
            MakeEmpty();
        }

        // QuadTree
        // Creates a QuadTree from a user-inputted image (2-d matrix) and sets properties.
        public QuadTree(int [,] Image, int size)
        {
            Node<T> root = new Node<T>(IsGray(Image));
            Root = root;
            Size = size;
            if (Root.Colour == 0)       // if the root is gray...
            {
                CreateQuad(Root, Image, Size);
            }
        }

        // CreateQuad
        // Creates a QuadTree from a user-inputted image (2-d matrix).
        private void CreateQuad(Node<T> curr_root, int[,] Image, int size)
        {
            int new_size = size / 2;

            // NORTH-WEST QUADRANT
            int[,] temp = NewImage(Image, 0, 0, new_size);
            int new_colour = IsGray(temp);
            Node<T> NW = new Node<T>(new_colour);
            curr_root.NW = NW;
            if (NW.Colour == 0)
            {
                CreateQuad(curr_root.NW, temp, new_size);
            }

            // NORTH-EAST QUADRANT
            temp = NewImage(Image, 0, new_size, new_size);
            new_colour = IsGray(temp);
            Node<T> NE = new Node<T>(new_colour);
            curr_root.NE = NE;
            if (NE.Colour == 0)
            {
                CreateQuad(curr_root.NE, temp, new_size);
            }

            // SOUTH-EAST QUADRANT
            temp = NewImage(Image, new_size, new_size, new_size);
            new_colour = IsGray(temp);
            Node<T> SE = new Node<T>(new_colour);
            curr_root.SE = SE;
            if (SE.Colour == 0)
            {
                CreateQuad(curr_root.SE, temp, new_size);
            }

            // SOUTH-WEST QUADRANT
            temp = NewImage(Image, new_size, 0, new_size);
            new_colour = IsGray(temp);
            Node<T> SW = new Node<T>(new_colour);
            curr_root.SW = SW;
            if (SW.Colour == 0)
            {
                CreateQuad(curr_root.SW, temp, new_size);
            }
        }

        // NewImage
        // Creates a new image representing the passed region of the current QuadTree
        private int[,] NewImage(int [,] Image, int startx, int starty, int size)
        {
            int[,] temp = new int[size, size];
            int i_count = 0, j_count;
            for (int i = startx; i_count < size; i++, i_count++)       // iterate through each pixel of the NEW image
            {
                j_count = 0;
                for (int j = starty; j_count < size; j++, j_count++)
                {
                    temp[i_count, j_count] = Image[i, j];       // add to the new image 2d-array
                }
            }
            return temp;
        }

        // IsGray
        // Determines whether the passed regiion of the current QuadTree is gray, white, or black.
        private int IsGray(int [,] Image)
        {
            int first = Image[0, 0];        // get the colour of the first pixel in the passed image 

            for (int i = 0; i < Image.GetLength(0); i++)        // iterate through each pixel of the image
            {
                for (int j = 0; j < Image.GetLength(1); j++)
                {
                    if (Image[i,j] != first)        // if not all pixels are the same colour...
                    {
                        return 0;       // return GRAY
                    }
                }
            }
            return first;       // return either BLACK or WHITE
        }

        // Switch
        // Calls private function of the same name to find and switch the pixel, then compresses the resultant tree.
        public void Switch(int i, int j)
        {
            Switch(i, j, 0, 0, Root, Size);
            Compress(Root);
        }

        // Switch
        // Finds desired pixel where indices i,j are located and changes the colour.
        private void Switch(int i, int j, int curr_i, int curr_j, Node<T> curr_node, int size)
        {
            if (curr_node.Colour != 0 && size != 1)     // if we need to enter a quadrant that is all the same colour
            {
                // create the new children for the current node
                Node<T> NW = new Node<T>(curr_node.Colour);
                Node<T> NE = new Node<T>(curr_node.Colour);
                Node<T> SE = new Node<T>(curr_node.Colour);
                Node<T> SW = new Node<T>(curr_node.Colour);

                // attach to parent
                curr_node.NW = NW;
                curr_node.NE = NE;
                curr_node.SE = SE;
                curr_node.SW = SW;

                curr_node.Colour = 0;       // make the current node gray
            }
            else if (curr_node.Colour != 0 && size == 1)        // if we have made it to the desired pixel
            {
                if (curr_node.Colour == 1)      // if it's white
                {
                    curr_node.Colour = 2;       // make it black
                }
                else        // if it's black
                {
                    curr_node.Colour = 1;       // make it white
                }
            }

            // NORTH-WEST QUADRANT
            if ((curr_node.Colour == 0) && (i >= curr_i) && (i < (curr_i + (size / 2))) &&      // if the desired pixel is in the NW child of current node
               (j >= curr_j) && (j < (curr_j + (size / 2))))
            {
                Switch(i, j, curr_i, curr_j, curr_node.NW, size / 2);
            }

            // NORTH-EAST QUADRANT
            if ((curr_node.Colour == 0) && (i >= curr_i) && (i < (curr_i + (size / 2))) &&      // if the desired pixel is in the NE child of current node
               (j >= (curr_j + (size / 2))) && (j < (curr_j + size)))
            {
                Switch(i, j, curr_i, curr_j + (size / 2), curr_node.NE, size / 2);
            }

            // SOUTH-EAST QUADRANT
            if ((curr_node.Colour == 0) && (i >= (curr_i + (size / 2))) && (i < (curr_i + size)) &&     // if the desired pixel is in the SE child of current node
               (j >= (curr_j + (size / 2))) && (j < (curr_j + size)))
            {
                Switch(i, j, curr_i + (size / 2), curr_j + (size / 2), curr_node.SE, size / 2);
            }

            // SOUTH-WEST QUADRANT
            if ((curr_node.Colour == 0) && (i >= (curr_i + (size / 2))) && (i < (curr_i + size)) &&     // if the desired pixel is in the SW child of current node
               (j >= curr_j) && (j < (curr_j + (size / 2))))
            {
                Switch(i, j, curr_i + (size / 2), curr_j, curr_node.SW, size / 2);
            }
        }

        // Compress
        // Compresses the current QuadTree via post-order traversal. If there is a node marked gray with children of all the same colour, remove the children and change it to white or black.
        private void Compress(Node<T> curr_root)
        {
            // NORTH-WEST QUADRANT
            if (curr_root.Colour == 0)
            {
                Compress(curr_root.NW);
            }

            // NORTH-EAST QUADRANT
            if (curr_root.Colour == 0)
            {
                Compress(curr_root.NE);
            }

            // SOUTH-EAST QUADRANT
            if (curr_root.Colour == 0)
            {
                Compress(curr_root.SE);
            }

            // SOUTH-WEST QUADRANT
            if (curr_root.Colour == 0)
            {
                Compress(curr_root.SW);
            }

            // COMPRESS NODE IF NEEDED
            if ((curr_root.Colour == 0) && (curr_root.NW.Colour == curr_root.NE.Colour) &&
                (curr_root.NW.Colour == curr_root.SE.Colour) && (curr_root.NW.Colour == curr_root.SW.Colour) &&
                (curr_root.NE.Colour == curr_root.SE.Colour) && (curr_root.NE.Colour == curr_root.SW.Colour) &&
                (curr_root.SE.Colour == curr_root.SW.Colour)) // if the node can be compressed...
            {
                curr_root.Colour = curr_root.NW.Colour;     // update colour from gray to either white or black
                curr_root.NW = null;        // remove children
                curr_root.NE = null;
                curr_root.SE = null;
                curr_root.SW = null;
            }
        }

        // Union
        // Calls private function of the same name to create a union of two quadtrees if both roots are gray. If not, return the appropriate existing QuadTree.
        public QuadTree<T> Union(QuadTree<T> Q)
        {
            MyGlobals.Union_Count = 0;
            QuadTree<T> New_QT = new QuadTree<T>();
            if (Root.Colour == Q.Root.Colour)       // if the root colours of both trees are the same
            {
                if (Root.Colour == 0)       // if the root colours of both trees are both gray
                {
                    Node<T> root = new Node<T>(0);
                    New_QT.Root = root;
                    Union(New_QT, Q, Root, Q.Root, New_QT.Root, Size, Q.Size);
                    double dbl_sqrt = Math.Sqrt(MyGlobals.Union_Count);
                    New_QT.Size = Convert.ToInt32(dbl_sqrt);
                    return New_QT;
                }
                else        // if the root colours of the trees are either both black or both white
                {
                    return this;        // return the first subtree
                }
            }
            else if (Root.Colour == 1 || Root.Colour == 2)      // if the root of the first subtree is white or black
            {
                return Q;       // return the second subtree
            }
            else        // if the root of the second subtree is white or black
            {
                return this;        // return the first subtree
            }
        }


        // Union
        // 
        private void Union(QuadTree<T> New_QT, QuadTree<T> Q, Node<T> curr_node1, Node<T> curr_node2, Node<T> new_node, int size1, int size2)
        {
            size1 /= 2;
            size2 /= 2;

            // COMPARING NODES
            if (curr_node1.Colour == 2 || curr_node2.Colour == 2)       // if one of or both of the current nodes of each tree are black
            {
                new_node.Colour = 2;
                MyGlobals.Union_Count += (size1 * size2);
            }
            else if (curr_node1.Colour == 1 && curr_node2.Colour == 1)      // if the current node of each tree are BOTH white
            {
                new_node.Colour = 1;
                MyGlobals.Union_Count += (size1 * size2);
            }
            else if (curr_node1.Colour == 1)        // if the current node of the first tree is white (and the other is gray)
            {
                new_node.NW = curr_node2.NW;
                new_node.NE = curr_node2.NE;
                new_node.SE = curr_node2.SE;
                new_node.SW = curr_node2.SW;
                MyGlobals.Union_Count += (size2 * size2);
            }
            else if (curr_node2.Colour == 1)        // if the current node of the second tree is white (and the other is gray)
            {
                new_node.NW = curr_node1.NW;
                new_node.NE = curr_node1.NE;
                new_node.SE = curr_node1.SE;
                new_node.SW = curr_node1.SW;
                MyGlobals.Union_Count += (size1 * size1);
            }

            // NORTH-WEST QUADRANT
            if (curr_node1.Colour == 0 && curr_node2.Colour == 0)
            {
                Node<T> new_NW = new Node<T>(0);
                new_node.NW = new_NW;
                Union(New_QT, Q, curr_node1.NW, curr_node2.NW, new_NW, size1, size2);
            }

            // NORTH-EAST QUADRANT
            if (curr_node1.Colour == 0 && curr_node2.Colour == 0)
            {
                Node<T> new_NE = new Node<T>(0);
                new_node.NE = new_NE;
                Union(New_QT, Q, curr_node1.NE, curr_node2.NE, new_NE, size1, size2);
            }

            // SOUTH-EAST QUADRANT
            if (curr_node1.Colour == 0 && curr_node2.Colour == 0)
            {
                Node<T> new_SE = new Node<T>(0);
                new_node.SE = new_SE;
                Union(New_QT, Q, curr_node1.SE, curr_node2.SE, new_SE, size1, size2);
            }

            // SOUTH-WEST QUADRANT
            if (curr_node1.Colour == 0 && curr_node2.Colour == 0)
            {
                Node<T> new_SW = new Node<T>(0);
                new_node.SW = new_SW;
                Union(New_QT, Q, curr_node1.SW, curr_node2.SW, new_SW, size1, size2);
            }
        }

        // MakeEmpty
        // Makes the current QuadTree empty
        public void MakeEmpty()
        {
            Root = null;
        }

        // Empty
        // Returns true if the current QuadTree is empty, false otherwise
        public bool Empty()
        {
            return Root == null;
        }

        // Print
        // Prints the current QuadTree
        public void Print()
        {
            string[,] arr = new string[Size, Size];

            // GETTING ORIGINAL ARRAY FROM QUADTREE
            if (Root.Colour == 0)       // if the root is gray
            {
                arr = Print_Traverse(Root, Size, 0, 0, arr);
            }
            else        // if the root is not gray (it is all one colour)
            {
                for (int i = 0; i < arr.GetLength(0); i++)        // iterate through each pixel of the image
                {
                    for (int j = 0; j < arr.GetLength(1); j++)
                    {
                            arr[i, j] = Enum.GetName(typeof(Node<T>.Colours), Root.Colour);     // fill the array with the same appropriate colour
                    }
                }
            }

            // PRINTING THE IMAGE
            for (int i = 0; i < arr.GetLength(0); i++)        // iterate through each pixel of the image
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    Console.Write(arr[i,j] + " ");          // print the current pixel
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        // Print_Traverse
        // Called from Print() to traverse the current QuadTree to fill a 2d-array with either W for White or B for Black and return that array
        private string[,] Print_Traverse(Node<T> curr_root, int size, int curr_i, int curr_j, string[,] arr)
        {
            //int new_size = size / 2;

            // NORTH-WEST QUADRANT
            if (curr_root.NW != null)       // go to NW of current node if not null
            {
                _ = Print_Traverse(curr_root.NW, size / 2, curr_i, curr_j, arr);
            }
            else
            {
                int i_count = 0, j_count;
                for (int i = curr_i; i_count < size; i++, i_count++)        // iterate through each pixel of the image
                {
                    j_count = 0;
                    for (int j = curr_j; j_count < size; j++, j_count++)
                    {
                        arr[i, j] = Enum.GetName(typeof(Node<T>.Colours), curr_root.Colour);        // add node to array the # of times to represent its region
                    }
                }
            }

            // NORTH-EAST QUADRANT
            if (curr_root.NE != null)       // go to NE of current node if not null
            {
                _ = Print_Traverse(curr_root.NE, size / 2, curr_i, curr_j + (size / 2), arr);
            }
            else
            {
                int i_count = 0, j_count;
                for (int i = curr_i; i_count < size; i++, i_count++)        // iterate through each pixel of the image
                {
                    j_count = 0;
                    for (int j = curr_j; j_count < size; j++, j_count++)
                    { 
                        arr[i, j] = Enum.GetName(typeof(Node<T>.Colours), curr_root.Colour);        // add node to array the # of times to represent its region
                    }
                }
            }

            // SOUTH-EAST QUADRANT
            if (curr_root.SE != null)       // go to SE of current node if not null
            {
                _ = Print_Traverse(curr_root.SE, size / 2, curr_i + (size / 2), curr_j + (size / 2), arr);
            }
            else
            {
                int i_count = 0, j_count;
                for (int i = curr_i; i_count < size; i++, i_count++)        // iterate through each pixel of the image
                {
                    j_count = 0;
                    for (int j = curr_j; j_count < size; j++, j_count++)
                    {
                        arr[i, j] = Enum.GetName(typeof(Node<T>.Colours), curr_root.Colour);        // add node to array the # of times to represent its region
                    }
                }
            }

            // SOUTH-WEST QUADRANT
            if (curr_root.SW != null)       // go to SW of current node if not null
            {
                _ = Print_Traverse(curr_root.SW, size / 2, curr_i + (size / 2), curr_j, arr);
            }
            else
            {
                int i_count = 0, j_count;
                for (int i = curr_i; i_count < size; i++, i_count++)        // iterate through each pixel of the image
                {
                    j_count = 0;
                    for (int j = curr_j; j_count < size; j++, j_count++)
                    {
                        arr[i, j] = Enum.GetName(typeof(Node<T>.Colours), curr_root.Colour);        // add node to array the # of times to represent its region
                    }
                }
            }

            // RETURNING ARRAY
            if (size == Size) // if we are returning to ORIGINAL call  
            {
                return arr;
            }
            else        // if we are returning to quadrant call, we don't care
            {
                return null;
            }
        }
    }

    // Program
    // Main driver for the program
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            QuadTree<int[,]> Q1 = new QuadTree<int[,]>();
            QuadTree<int[,]> Q2 = new QuadTree<int[,]>();
            double dbl_size;
            int int_choice, int_colour_choice, int_quad_choice, int_size, int_size_choice, i_choice, j_choice;
            MyGlobals.EXIT = false;

            while (!MyGlobals.EXIT)
            {
                // visual menu
                MyGlobals.ERROR = true;
                Console.WriteLine("******************************************************");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("*                     QUADTREES                      *");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("******************************************************");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("*    1 = CREATE A NEW QUADTREE                       *");
                Console.WriteLine("*    2 = RESET A QUADTREE                            *");
                Console.WriteLine("*    3 = SWITCH A COLOUR                             *");
                Console.WriteLine("*    4 = UNION QUADTREES                             *");
                Console.WriteLine("*    5 = PRINT A QUADTREE                            *");
                Console.WriteLine("*    6 = EXIT PROGRAM                                *");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("******************************************************\n");

                int_choice = program.IntChoice();       // get user's menu choice

                switch (int_choice)
                {
                    // CREATE A NEW QUADTREE
                    case 1:
                        // QUADTREE CHOICE
                        int_quad_choice = program.QuadTreeChoice();     // get user's choice of quadtree
                        if (int_quad_choice != 1 && int_quad_choice != 2)
                        {
                            Console.WriteLine("ERROR: QuadTree does not exist. Returning to main menu...\n");       // print error message
                            break;
                        }

                        // IMAGE SIZE
                        Console.WriteLine("Please specify the size of your image. The size will be 2 to the power of your input...");
                        Console.WriteLine("i.e. if you enter 3, your image's size will be 2^3 = 8.");
                        int_size_choice = program.IntChoice();      // get user-inputted size of image
                        dbl_size = Math.Pow(2, int_size_choice);        // convert input size as a power to the base of 2
                        int_size = Convert.ToInt32(dbl_size);       // convert result back to integer
                        int[,] arr = new int[int_size, int_size];       // create new 2-d array of user-specified size to hold "pixels"

                        // FILLING ARRAY
                        Console.WriteLine("Please choose the colour of each pixel in your image. Choose 1 for White or 2 for Black...");
                        Console.WriteLine("We will go from left to right, top to bottom (like how you read).");
                        for (int i = 0; i < int_size; i++)      // traverse 2-d array "arr"
                        {
                            for (int j = 0; j < int_size; j++)
                            {
                                do
                                {
                                    Console.WriteLine("1 or 2?");
                                    int_colour_choice = program.IntChoice();        // get user's colour choice
                                    if (int_colour_choice == 1 || int_colour_choice == 2)       // if white or black
                                    {
                                        arr[i, j] = int_colour_choice;      // insert 1 or 2 into current position in arr
                                    }
                                    else
                                    {
                                        Console.WriteLine("ERROR: Invalid option. Please try again.\n");        // print error message
                                    }
                                } while (int_colour_choice != 1 && int_colour_choice != 2);
                            }
                        }

                        // PASSING TO CONSTRUCTOR
                        switch (int_quad_choice)
                        {
                            case 1:
                                QuadTree<int[,]> Q_1 = new QuadTree<int[,]>(arr, int_size);     // create quadtree #1
                                Q1 = Q_1;
                                Console.WriteLine("QuadTree #1 created successfully.\n");
                                break;
                            case 2:
                                QuadTree<int[,]> Q_2 = new QuadTree<int[,]>(arr, int_size);     // create quadtree #2
                                Q2 = Q_2;
                                Console.WriteLine("QuadTree #2 created successfully.\n");
                                break;
                            default:
                                Console.WriteLine("ERROR: QuadTree does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        break;

                    // RESET A QUADTREE
                    case 2:
                        int_quad_choice = program.QuadTreeChoice();     // get user's choice of quadtree
                        switch (int_quad_choice)
                        {
                            case 1:
                                Q1.MakeEmpty();     // empty quadtree #1
                                Console.WriteLine("QuadTree #1 reset successfully.\n");
                                break;
                            case 2:
                                Q2.MakeEmpty();     // empty quadtree #2
                                Console.WriteLine("QuadTree #2 reset successfully.\n");
                                break;
                            default:
                                Console.WriteLine("ERROR: QuadTree does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        break;

                    // SWITCH A COLOUR
                    case 3:
                        // QUADTREE CHOICE
                        int_quad_choice = program.QuadTreeChoice();     // get user's choice of quadtree
                        if (int_quad_choice != 1 && int_quad_choice != 2)       // if the choice of quadtree is invalid
                        {
                            Console.WriteLine("ERROR: QuadTree does not exist. Returning to main menu...\n");       // print error message
                            break;
                        }

                        // INDEXES
                        Console.WriteLine("Please specify the first index of the pixel you want to switch...");
                        i_choice = program.IntChoice();     // get user input of index i
                        Console.WriteLine("Please specify the second index of the pixel you want to switch...");
                        j_choice = program.IntChoice();     // get user input of index j

                        // PASSING TO CONSTRUCTOR
                        switch (int_quad_choice)
                        {
                            case 1:
                                if (Q1.Empty())     // if quadtree #1 is empty
                                {
                                    Console.WriteLine("ERROR: QuadTree is empty. Returning to main menu...\n");     // print error message
                                    break;
                                }
                                else if (i_choice < 0 || i_choice >= Q1.Size || j_choice < 0 || j_choice >= Q1.Size)        // if either index is out of range
                                {
                                    Console.WriteLine("ERROR: Index(es) out of range. Returning to main menu...\n");        // print error message
                                    break;
                                }
                                Q1.Switch(i_choice, j_choice);      // call function to perform the colour switch
                                Console.WriteLine("QuadTree #1 After Switch:\n");
                                Q1.Print();     // print the resultant quadtree
                                break;
                            case 2:
                                if (Q2.Empty())     // if quadtree #2 is empty
                                {
                                    Console.WriteLine("ERROR: QuadTree is empty. Returning to main menu...\n");     // print error message
                                    break;
                                }
                                else if (i_choice < 0 || i_choice >= Q2.Size || j_choice < 0 || j_choice >= Q2.Size)        // if either index is out of range
                                {
                                    Console.WriteLine("ERROR: Index(es) out of range. Returning to main menu...\n");        // print error message
                                    break;
                                }
                                Q2.Switch(i_choice, j_choice);      // call function to perform the colour switch
                                Console.WriteLine("QuadTree #2 After Switch:\n");
                                Q2.Print();     // print the resultant quadtree
                                break;
                            default:
                                Console.WriteLine("ERROR: QuadTree does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        break;

                    // UNION QUADTREES
                    case 4:
                        if (Q1.Empty() || Q2.Empty())       // if either of the quadtrees are empty
                        {
                            Console.WriteLine("ERROR: One or both of the QuadTrees are empty. Returning to main menu...\n");        // print error message
                            break;
                        }
                        QuadTree<int[,]> Q3 = Q1.Union(Q2);     // call function to perform the union of the quadtrees
                        Console.WriteLine("Union of QuadTrees:\n");
                        Q3.Print();     // print the resultant tree
                        break;
                    
                    // PRINT A QUADTREE
                    case 5:
                        int_quad_choice = program.QuadTreeChoice();     // get user's choice of quadtree
                        switch (int_quad_choice)
                        {
                            case 1:
                                if (Q1.Empty())     // if quadtree #1 is empty
                                {
                                    Console.WriteLine("ERROR: QuadTree is empty. Returning to main menu...\n");     // print error message
                                    break;
                                }
                                Q1.Print();     // print quadtree #1
                                break;
                            case 2:
                                if (Q2.Empty())     // if quadtree #2 is empty
                                {
                                    Console.WriteLine("ERROR: QuadTree is empty. Returning to main menu...\n");     // print error message
                                    break;
                                }
                                Q2.Print();     // print quadtree #2
                                break;
                            default:
                                Console.WriteLine("ERROR: QuadTree does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        Console.WriteLine("\n");
                        break;

                    // EXIT PROGRAM
                    case 6:
                        MyGlobals.EXIT = true;
                        Console.WriteLine("Exiting program...");
                        Environment.Exit(0);
                        break;

                    // INVALID INTEGER MENU CHOICE
                    default:
                        Console.WriteLine("ERROR: Invalid option. Please try again.\n");        // print error message
                        break;
                }
            }
        }

        // IntChoice
        // Gets user input as integer and ensures input is in fact an integer.
        public int IntChoice()
        {
            bool conversion;
            int int_choice;
            string choice;

            do
            {
                choice = Console.ReadLine();        // get user input
                conversion = Int32.TryParse(choice, out int_choice);        // ensure input is an integer
                if (!conversion)        // if input is not an integer...
                {
                    Console.WriteLine("ERROR: Invalid option. Please try again.\n");         // print error message
                }
                else        // if input is an integer...
                {
                    MyGlobals.ERROR = false;        // not an error
                    break;
                }
                Console.WriteLine();
            } while (MyGlobals.ERROR);
            MyGlobals.ERROR = true;
            Console.WriteLine();
            return int_choice;
        }

        // QuadTreeChoice
        // Gets user input as integer representing which QuadTree user would like to work with and ensures input is in fact an integer.
        public int QuadTreeChoice()
        {
            bool conversion;
            int int_quad_choice;
            string quad_choice;

            Console.WriteLine("Which QuadTree would you like to use? (1 or 2)");
            do
            {
                quad_choice = Console.ReadLine();       // get user input
                conversion = Int32.TryParse(quad_choice, out int_quad_choice);      // ensure input is an integer
                if (!conversion)        // if input is not an integer...
                {
                    Console.WriteLine("ERROR: Invalid option. Please try again.\n");        // print error message
                }
                else        // if input is an integer...
                {
                    MyGlobals.ERROR = false;        // not an error
                    break;
                }
                Console.WriteLine();
            } while (MyGlobals.ERROR);
            MyGlobals.ERROR = true;
            Console.WriteLine();
            return int_quad_choice;
        }
    }
}
