using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField] private int _recursionDepth;
    public static AI Instance;
    private int _width;
    private int _height;
    

    private void Awake()
    {
        Instance = this;
    }

    /**
     * method responsible for the Ai to take a turn
     */
    public void TakeTurn()
    {
        //temp
        _width = GridManager.Instance.GetWidth();
        _height = GridManager.Instance.GetHeight();

        //gather resources
        bool isAiTurn = PlayerTurnManager.Instance.PlayerTurn == Team.playerOne;

        //build tree
        /**
        * boardState is a 2d array of int where 0 is an empty tile, 
        * 1 is a friendly piece, and 2 is an opposition piece
        * 3 is a friendly king, and 4 is an opposition king
        */
        int[,] currentState = ConvertBoardState();
        TreeNode rootBehaviourNode = new TreeNode(currentState, EvaluationFunction(currentState));
        BuildTree(_recursionDepth, rootBehaviourNode, isAiTurn);

        //minmax
        MinMax(rootBehaviourNode, _recursionDepth, isAiTurn);

        //next turn
        PlayerTurnManager.Instance.NextTurn();
    }

    /**
     * recursive method for creating a node
     */
    private void BuildTree(int currentDepth, TreeNode prevNode, bool isAiTurn)
    {
        //end state
        if (currentDepth == 0) return;

        //find next moves
        prevNode.SetBranches(new List<TreeNode>());
        int[,] state = prevNode.GetState();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                //iterate through each tile
                int t = state[x, y];

                //continue if empty tile, or if one cannot control the piece in current turn
                if (t == 0) continue;
                if (!isAiTurn && (t == 1 || t == 3) || isAiTurn && (t == 2 || t == 4)) continue;

                //iterate through possible moves and changes to the board state
                foreach (int[,] newState in GetPossibleStates(isAiTurn, x, y, state))
                {
                    //create new branches
                    TreeNode newBranch = new TreeNode(newState, EvaluationFunction(newState));
                    prevNode.GetBranches().Add(newBranch);
                    BuildTree(currentDepth - 1, newBranch, !isAiTurn);
                }
            }
        }
    }

    /**
     * minmax algorithms to guide ai decisions
     */
    private TreeNode MinMax(TreeNode node, int depth, bool maximisingPlayer)
    { 
        //end state
        if (depth == 0) return node;
        if (node.GetBranches().Count == 0) return node;

        //minmax
        if (maximisingPlayer)
        {
            //max side
            int maxEval = -int.MaxValue;
            TreeNode maxNode = null;
            foreach (TreeNode child in node.GetBranches())
            {
                TreeNode nodeBelow = MinMax(child, depth - 1, false);
                int v = nodeBelow.GetValue();
                maxEval = maxEval > v ? maxEval : v;
                maxNode = maxEval > v ? maxNode : nodeBelow;
            }
            return maxNode;
        }
        else
        {
            //min side
            int minEval = int.MaxValue;
            TreeNode minNode = null;
            foreach (TreeNode child in node.GetBranches())
            {
                TreeNode nodeBelow = MinMax(child, depth - 1, true);
                int v = nodeBelow.GetValue();
                minEval = minEval < v ? minEval : v;
                minNode = minEval < v ? minNode : nodeBelow;
            }
            return minNode;
        }
    }

    /**
     * take whether its the ai's turn, a state, and the position of a piece in the state and 
     * return all new states than can be found from moving that piece
     */
    private List<int[,]> GetPossibleStates(bool isAiTurn, int x, int y, int[,] state)
    {
        //stores new states in this
        List<int[,]> newStates = new List<int[,]>();

        //find transforms for piece
        int[][] moves = GetValidTransforms(state[x,y], isAiTurn); 

        //examine tiles
        foreach (int[] transform in moves)
        {
            //check if move goes outside of board
            if (!(x + transform[0] >= 0 && x + transform[0] < _width
                && y + transform[1] >= 0 && y + transform[1] < _height)) continue;

            //get move after transform
            int examineTileValue = state[x + transform[0], y + transform[1]];

            //if tile is empty, can move to
            if (examineTileValue == 0)
                newStates.Add(MorphState(x, y, transform, state, "move"));

            //if tile has enemy piece
            if ( examineTileValue == 2 || examineTileValue == 4)
                //if tile otherside of enemy piece is on board
                if (x + transform[0]*2 >= 0 && x + transform[0]*2 < _width
                    && y + transform[1]*2 >= 0 && y + transform[1]*2 < _height)
                    //if if tile otherside of enemy piece is empty
                    if (state[x + transform[0]*2, y + transform[1]*2] == 0)
                        newStates.Add(MorphState(x, y, transform, state, "take"));
        }

        return newStates;
    }

    /**
     * changes a given state matrix to reflect a hypothesised move
     * x and y are the cartesian position of the tile to be moved
     * transform is the translation to move the piece
     * old state is the state to be morphed
     * moveType is the type of move the piece will do
     */
    private int[,] MorphState(int x, int y, int[] transform, int[,] state, string moveType)
    {
        switch (moveType)
        {
            case "move":
                state[x + transform[0], y + transform[1]] = state[x, y];
                break;
            case "take":
                state[x + transform[0] * 2, y + transform[1] * 2] = state[x, y];
                state[x + transform[0], y + transform[1]] = 0;
                break;
        }            
        state[x, y] = 0;
        return state;
    }

    /**
    * holds the valid move tranforms
    */
    private int[][] GetValidTransforms(int type, bool isAiTurn)
    {
        List<int[]> moves = new List<int[]>();
        moves.Add(new int[] {-1, isAiTurn ? 1 : -1});
        moves.Add(new int[] { 1, isAiTurn ? 1 : -1});
        if(type == 3 || type == 4)
        {
            moves.Add(new int[] {-1, isAiTurn ? -1 : 1});
            moves.Add(new int[] { 1, isAiTurn ? -1 : 1});
        }
        return moves.ToArray();
    }

    /**
    * converts the actual board into the boardState notation
    */
    public int[,] ConvertBoardState()
    {
        int[,] boardState = new int[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile t = GridManager.Instance.GetTiles()[new Vector2(x, y)];
                boardState[x, y] = FindTileState(t);
            }
        }
        return boardState;
    }

    /**
     * finds the state of a tile
     */
    private int FindTileState(Tile t)
    {
        Piece p = t.GetOccupiedBy();
        if (p == null) return 0;
        else
        {
            if (p.GetTeam() == Team.playerOne) return p.IsKing() ? 3 : 1;
            else return p.IsKing() ? 4 : 2;

        }
    }

    /**
     * evaluates the board state using a heuristic and returns a value
     * the higher the better
     * 
     * current evaluation function adds all remaining white pieces and negates 
     * remaining black pieces
     */
    private int EvaluationFunction(int[,] state)
    {
        int value = 0;
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (state[x, y] == 1 || state[x, y] == 3)
                {
                    value += 1;
                }
                else if (state[x, y] == 2 || state[x, y] == 4)
                {
                    value -= 1;
                }
            }
        }
        return value;
    }
}
