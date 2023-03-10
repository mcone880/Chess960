using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public string gameMode = "";
	public GameObject CubeDark;
	public GameObject CubeLight;
	public GameObject[] PiecesGO = new GameObject[6];
	public int activePlayer = 1;  // 1 = White, -1 = Dark
	public bool player1AI = false;  // Bool that state if player1 is a AI
	public bool player2AI = false;  // Bool that state if player2 is a AI
	public Material DarkMat;
	public Material LightMat;
	public int gameState = 0;           // In this state, the code is waiting for : 0 = Piece selection, 1 = Piece animation

	Dictionary<string, (GameObject, int)> whitePieces = new Dictionary<string, (GameObject, int)>();
	Dictionary<string, (GameObject, int)> blackPieces = new Dictionary<string, (GameObject, int)>();

	private GameObject SelectedPiece;	// Selected Piece
	private int _boardHeight = -1;
	private int _pieceHeight =  0;
	private static int _boardSize   =  8;
	private int[,] _boardPieces = new int[_boardSize,_boardSize];
	void OnGUI()
	{
		string _activePlayerColor;
		if(activePlayer == 1)
			_activePlayerColor = "White";
		else
			_activePlayerColor = "Dark";
		
		
		GUI.Label (new Rect(10,10,200,20), ("Active Player = " + _activePlayerColor));	
		GUI.Label (new Rect(10,30,200,20), ("Game State = " + gameState));
	}
	
	// Initialize the board area
	void Start()
	{
		CreateBoard();
		AddPieces();
	}
	
	// Create the board by placing cubes
	void CreateBoard()
	{
		for(int i = 0; i < _boardSize; i++)
		{
			for(int j = 0; j < _boardSize; j++)
			{
				if((i+j)%2 == 0)
				{
					Object.Instantiate(CubeDark,new Vector3(i,_boardHeight,j), Quaternion.identity);	
				}
				else
				{
					Object.Instantiate(CubeLight,new Vector3(i,_boardHeight,j), Quaternion.identity);
				}
			}
		}
	}
	
	// Add all pieces are their respective position
	void AddPieces()
	{
		const int blackLinePos = 7;
		const int blackPlayer = -1;
		const int whiteLinePos = 0;
		const int whitePlayer = 1;
		
		// Create all pawn at once
		for(int i = 0; i < _boardSize; i++)
		{
			CreatePiece("Pawn", i, 1, 1); // Create all white pawn 	
			CreatePiece("Pawn", i, 6, -1); // Create all dark pawn
		}

		if(PlayerPrefs.GetString("GameMode") == "Regular Chess")
        {
			// Create white pieces
			CreatePiece("Rook1", 0, whiteLinePos, whitePlayer);
			CreatePiece("Knight1", 1, whiteLinePos, whitePlayer);
			CreatePiece("Bishop1", 2, whiteLinePos, whitePlayer);
			CreatePiece("Queen", 3, whiteLinePos, whitePlayer);
			CreatePiece("King", 4, whiteLinePos, whitePlayer);
			CreatePiece("Bishop2", 5, whiteLinePos, whitePlayer);
			CreatePiece("Knight2", 6, whiteLinePos, whitePlayer);
			CreatePiece("Rook2", 7, whiteLinePos, whitePlayer);

			// Create Dark pieces
			CreatePiece("Rook1", 0, blackLinePos, blackPlayer);
			CreatePiece("Knight1", 1, blackLinePos, blackPlayer);
			CreatePiece("Bishop1", 2, blackLinePos, blackPlayer);
			CreatePiece("Queen", 3, blackLinePos, blackPlayer);
			CreatePiece("King", 4, blackLinePos, blackPlayer);
			CreatePiece("Bishop2", 5, blackLinePos, blackPlayer);
			CreatePiece("Knight2", 6, blackLinePos, blackPlayer);
			CreatePiece("Rook2", 7, blackLinePos, blackPlayer);
		} else
        {
			//Create White Pieces
			bool bishop1Even = false;
			List<int> takenSpots = new List<int>();

			int randPlace = Random.Range(1, 7);
			CreatePiece("King", randPlace, whiteLinePos, whitePlayer);
			CreatePiece("King", randPlace, blackLinePos, blackPlayer);
			takenSpots.Add(randPlace);
			int kingPos = randPlace;

			do
			{
				randPlace = Random.Range(0, kingPos);
			} while (takenSpots.Contains(randPlace));
			CreatePiece("Rook1", randPlace, whiteLinePos, whitePlayer);
			CreatePiece("Rook1", randPlace, blackLinePos, blackPlayer);
			takenSpots.Add(randPlace);

			do
			{
				randPlace = Random.Range(kingPos, 8);
			} while (takenSpots.Contains(randPlace));
			CreatePiece("Rook2", randPlace, whiteLinePos, whitePlayer);
			CreatePiece("Rook2", randPlace, blackLinePos, blackPlayer);
			takenSpots.Add(randPlace);

			do
			{
				randPlace = Random.Range(0, 7);
			} while (takenSpots.Contains(randPlace));
			CreatePiece("Bishop1", randPlace, whiteLinePos, whitePlayer);
			CreatePiece("Bishop1", randPlace, blackLinePos, blackPlayer);
			takenSpots.Add(randPlace);
			if (randPlace % 2 == 0) bishop1Even = true;
			if (bishop1Even)
			{
				do
				{
					randPlace = Random.Range(0, 8);
				} while (randPlace % 2 == 0 || takenSpots.Contains(randPlace));
			}
			else
			{
				do
				{
					randPlace = Random.Range(0, 8);
				} while (randPlace % 2 != 0 || takenSpots.Contains(randPlace));
			}
			CreatePiece("Bishop2", randPlace, whiteLinePos, whitePlayer);
			CreatePiece("Bishop2", randPlace, blackLinePos, blackPlayer);
			takenSpots.Add(randPlace);

			do
			{
				randPlace = Random.Range(0, 8);
			} while (takenSpots.Contains(randPlace));
			CreatePiece("Knight1", randPlace, whiteLinePos, whitePlayer);
			CreatePiece("Knight1", randPlace, blackLinePos, blackPlayer);
			takenSpots.Add(randPlace);

			do
			{
				randPlace = Random.Range(0, 8);
			} while (takenSpots.Contains(randPlace));
			CreatePiece("Knight2", randPlace, whiteLinePos, whitePlayer);
			CreatePiece("Knight2", randPlace, blackLinePos, blackPlayer);
			takenSpots.Add(randPlace);

            do
            {
                randPlace = Random.Range(0, 8);
            } while (takenSpots.Contains(randPlace));
            CreatePiece("Queen", randPlace, whiteLinePos, whitePlayer);
            CreatePiece("Queen", randPlace, blackLinePos, blackPlayer);

			RunUnitTests(bishop1Even);
        }
	}

	public void RunUnitTests(bool bishop1)
    {
		//Should Succeed
		RunKingCheck(whitePieces);
		RunKingCheck(blackPieces);
		RunBishopCheck(whitePieces, bishop1);
		RunBishopCheck(blackPieces, bishop1);

		//Should Fail
		RunKingPosition(whitePieces);
		RunKingPosition(blackPieces);
    }

	public void RunKingCheck(Dictionary<string, (GameObject, int)> pieces)
    {
		var king = pieces["King"];
		var rook1 = pieces["Rook1"];
		var rook2 = pieces["Rook2"];

		print("King Test: " + (king.Item2 > rook1.Item2 && king.Item2 < rook2.Item2));
    }

	public void RunKingPosition(Dictionary<string, (GameObject, int)> pieces)
    {
		var king = pieces["King"];
		print("King Pos Test: " + (king.Item2 == 7 || king.Item2 == 0));
    }

	public void RunBishopCheck(Dictionary<string, (GameObject, int)> pieces, bool bish1Even)
    {
		var bis1 = pieces["Bishop1"];
		var bis2 = pieces["Bishop2"];

		if(bish1Even)
        {
			print("Bishop Test: " + (bis1.Item2 % 2 == 0 && bis2.Item2 % 2 != 0));
        } else
        {
			print("Bishop Test: " + (bis2.Item2 % 2 == 0 && bis1.Item2 % 2 != 0));
		}
    }



	
	// Spawn a piece on the board
	void CreatePiece(string _pieceName, int _posX, int _posY, int _playerTag)
	{
		GameObject _PieceToCreate = null;
		int 	   _pieceIndex = 0;
		//Select the right prefab to instantiate
		switch (_pieceName)
		{
		    case "Pawn": 
				_pieceIndex = 1;
		        break;
			case "Rook1": 
				_pieceIndex = 2;
		        break;
			case "Rook2":
				_pieceIndex = 2;
				break;
			case "Knight1": 
				_pieceIndex = 3;
		        break;
			case "Knight2":
				_pieceIndex = 3;
				break;
			case "Bishop1": 
				_pieceIndex = 4;
		        break;
			case "Bishop2":
				_pieceIndex = 4;
				break;
			case "Queen": 
				_pieceIndex = 5;
		        break;
			case "King": 
				_pieceIndex = 6;
		        break;
		}
		_PieceToCreate = PiecesGO[_pieceIndex-1];
		// Instantiate the piece as a GameObject to be able to modify it after
		_PieceToCreate = Object.Instantiate (_PieceToCreate,new Vector3(_posX, _pieceHeight, _posY), Quaternion.identity) as GameObject;
		_PieceToCreate.name = _pieceName;
		
		//Add material to the piece and tag it
		if(_playerTag == 1)
		{
			_PieceToCreate.tag = "1";
			_PieceToCreate.GetComponent<Renderer>().material = LightMat;
			if(_PieceToCreate.name != "Pawn") whitePieces.Add(_PieceToCreate.name, (_PieceToCreate, _posX));
		}
		else if(_playerTag == -1)
		{
			_PieceToCreate.tag = "-1";
			_PieceToCreate.GetComponent<Renderer>().material = DarkMat;
			if (_PieceToCreate.name != "Pawn") blackPieces.Add(_PieceToCreate.name, (_PieceToCreate, _posX));
		}

		_boardPieces[_posX,_posY] = _pieceIndex*_playerTag; 
	}
	
	//Update SlectedPiece with the GameObject inputted to this function
	public void SelectPiece(GameObject _PieceToSelect)
	{
		// Unselect the piece if it was already selected
		if(_PieceToSelect  == SelectedPiece)
		{
			SelectedPiece.GetComponent<Renderer>().material.color = Color.white;
			SelectedPiece = null;
			ChangeState (0);
		}
		else
		{
			// Change color of the selected piece to make it apparent. Put it back to white when the piece is unselected
			if(SelectedPiece)
			{
				SelectedPiece.GetComponent<Renderer>().material.color = Color.white;
			}
			SelectedPiece = _PieceToSelect;
			SelectedPiece.GetComponent<Renderer>().material.color = Color.blue;
			ChangeState (1);
		}
	}
	
	// Move the SelectedPiece to the inputted coords
	public void MovePiece(Vector2 _coordToMove)
	{
		bool validMovementBool = false;
		Vector2 _coordPiece = new Vector2(SelectedPiece.transform.position.x, SelectedPiece.transform.position.z);
		
		// Don't move if the user clicked on its own cube or if there is a piece on the cube
		if((_coordToMove.x != _coordPiece.x || _coordToMove.y != _coordPiece.y) || _boardPieces[(int)_coordToMove.x,(int)_coordToMove.y] != 0)
		{
			validMovementBool	= TestMovement (SelectedPiece, _coordToMove);
		}
		
		if(validMovementBool)
		{
			_boardPieces[(int)_coordToMove.x, (int)_coordToMove.y] = _boardPieces[(int)_coordPiece.x, (int)_coordPiece.y];
			_boardPieces[(int)_coordPiece.x , (int)_coordPiece.y ] = 0;
			
			SelectedPiece.transform.position = new Vector3(_coordToMove.x, _pieceHeight, _coordToMove.y);		// Move the piece
			SelectedPiece.GetComponent<Renderer>().material.color = Color.white;	// Change it's color back
			SelectedPiece = null;									// Unselect the Piece
			ChangeState (0);
			activePlayer = -activePlayer;
		}
	}
	
	// If the movement is legal, eat the piece
	public void EatPiece(GameObject _PieceToEat)
	{
		Vector2 _coordToEat = new Vector2(_PieceToEat.transform.position.x, _PieceToEat.transform.position.z);
		int _deltaX = (int)(_PieceToEat.transform.position.x - SelectedPiece.transform.position.x);
		
		if(TestMovement (SelectedPiece, _coordToEat) && (SelectedPiece.name != "Pawn" ||  _deltaX != 0))
		{
			Object.Destroy (_PieceToEat);
			_boardPieces[(int)_coordToEat.x, (int)_coordToEat.y] = 0; 
			MovePiece(_coordToEat);
		}
	}
	
	// Test if the piece can do the player's movement
	bool TestMovement(GameObject _SelectedPiece, Vector2 _coordToMove)
	{
		bool _movementLegalBool = false;
		bool _collisionDetectBool = false;
		Vector2 _coordPiece = new Vector2(_SelectedPiece.transform.localPosition.x, _SelectedPiece.transform.localPosition.z);
		
		int _deltaX = (int)(_coordToMove.x - _coordPiece.x);
		int _deltaY = (int)(_coordToMove.y - _coordPiece.y);
		int activePlayerPawnPostion = 1;
		//Debug.Log("Piece (" + _coordPiece.x + "," + _coordPiece.x + ") - Move (" + _coordToMove.x + "," + _coordToMove.y + ")");
		//Debug.Log("Delta (" + _deltaX + "," + _deltaY + ")");
		// Use the name of the _SelectedPiece GameObject to find the piece used
		switch (_SelectedPiece.name)
		{
			
		    case "Pawn": 
				if(activePlayer == -1)
					activePlayerPawnPostion = 6;		
			
			
				// Pawn can move 1 steap ahead of them, 2 if they are on their initial position
		        if(_deltaY == activePlayer || (_deltaY == 2*activePlayer && _coordPiece.y == activePlayerPawnPostion))
				{
				//Debug.Log ("_deltaY");
					_movementLegalBool = true;
				}
				
		        break;
		    case "Rook1":
				// Rook can move horizontally or vertically
				if((_deltaX != 0 && _deltaY == 0) || (_deltaX == 0 && _deltaY != 0)) 
				{
					_movementLegalBool = true;
				}
		        break;
			case "Rook2":
				// Rook can move horizontally or vertically
				if ((_deltaX != 0 && _deltaY == 0) || (_deltaX == 0 && _deltaY != 0))
				{
					_movementLegalBool = true;
				}
				break;
			case "Knight1":
				// Knight move in a L movement. distance is evaluated by a multiplication of both direction
				if((_deltaX != 0 && _deltaY != 0) && Mathf.Abs(_deltaX*_deltaY) == 2)
				{
					_movementLegalBool = true;
				}
				break;
			case "Knight2":
				// Knight move in a L movement. distance is evaluated by a multiplication of both direction
				if ((_deltaX != 0 && _deltaY != 0) && Mathf.Abs(_deltaX * _deltaY) == 2)
				{
					_movementLegalBool = true;
				}
				break;
			case "Bishop1":
				// Bishop can only move diagonally
				if(Mathf.Abs(_deltaX/_deltaY) == 1)
				{
					_movementLegalBool = true;
				}
		        break;
			case "Bishop2":
				// Bishop can only move diagonally
				if (Mathf.Abs(_deltaX / _deltaY) == 1)
				{
					_movementLegalBool = true;
				}
				break;
			case "Queen":
				// Queen movement is a combination of Rook and bishop
				if(((_deltaX != 0 && _deltaY == 0) || (_deltaX == 0 && _deltaY != 0)) || Mathf.Abs(_deltaX/_deltaY) == 1)
				{
					_movementLegalBool = true;
				}
		        break;
			
			case "King":
				// Bishop can only move diagonally
				if(Mathf.Abs(_deltaX + _deltaY) == 1) 
				{
					_movementLegalBool = true;
				}
		        break;
			
		    default:
		        _movementLegalBool = false;
		        break;
		}
		
		// If the movement is legal, detect collision with piece in the way. Don't do it with knight since they can pass over pieces.
		if(_movementLegalBool && SelectedPiece.name != "Knight")
		{
			_collisionDetectBool = TestCollision (_coordPiece, _coordToMove);
		}
			
		return (_movementLegalBool && !_collisionDetectBool);
	}
	
	// Test if a unit is in the path of the tested movement
	bool TestCollision(Vector2 _coordInitial,Vector2 _coordFinal)
	{
		bool CollisionBool = false;
		int _deltaX = (int)(_coordFinal.x - _coordInitial.x);
		int _deltaY = (int)(_coordFinal.y - _coordInitial.y);
		int _incX = 0; // Direction of the incrementation in X
		int _incY = 0; // Direction of the incrementation in Y
		int i;
		int j;
		
		// Calculate the increment if _deltaX/Y is different from 0 to avoid division by 0
		if(_deltaX != 0)
		{
			_incX = (_deltaX/Mathf.Abs(_deltaX));
		}
		if(_deltaY != 0)
		{
			_incY = (_deltaY/Mathf.Abs(_deltaY));
		}
		
		i = (int)_coordInitial.x + _incX;
		j = (int)_coordInitial.y + _incY;
		
		while(new Vector2(i, j) != _coordFinal)
		{
					
			if(_boardPieces[i,j] != 0)
			{
				CollisionBool = true;
				break;
			}
			
			i += _incX;
			j += _incY;
		}
		Debug.Log (CollisionBool);
		return CollisionBool;
	}

	// Change the state of the game
	public void ChangeState(int _newState)
	{
		gameState = _newState;
	}
}
