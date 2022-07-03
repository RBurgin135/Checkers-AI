using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreeNode
{
    //state of the board
    private int[,] state;
    //links to lower nodes in the tree
    private List<TreeNode> branches;
    //evaluation value for the board state
    private int value;

    /**
     * constructor
     */
    public TreeNode(int[,] state, int value)
    {
        this.value = value;
        this.state = state;
    }

    /**
     * getter for state
     */
    public int[,] GetState()
    {
        return state;
    }

    /**
     * getter for value
     */
    public int SetValue()
    {
        return value;
    }

    /**
    * adds a new node to the tree
    */
    public void AddBranch(TreeNode x)
    {
        branches.Add(x);
    }
}
