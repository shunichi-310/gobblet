using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 駒の所属チームを表す列挙型
public enum PieceTeam
{
    Blue,    // 青チーム
    Orange   // オレンジチーム
}

// 駒を表すクラス
public class Piece : MonoBehaviour
{
    public PieceTeam team;   // 駒の所属チーム（青、オレンジ）
    public Material MaterialBlue; // ブルーマテリアル
    public Material MaterialOrange;//オレンジマテリアル
    public Material MaterialGreen; // 選択時のマテリアル
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip missSound;
    public AudioClip getSound;

    private bool isGrabbing; // マウスがつかんでいるかどうかのフラグ
    private GameManager gameManager; // GameManagerへの参照

    Plane plane;             // マウスクリック時に生成される平面
    Transform sphere;          // つかんでいるオブジェクトのTransform
    Transform selectedPiece = null; // 選択された駒を保存する変数

    // Start is called before the first frame update
    void Start()
    {
        // 平面の定義：法線ベクトル(Vector3.up)がy軸方向で、位置(Vector3.up)が原点上にある平面
        plane = new Plane(Vector3.up, Vector3.up);

        // GameManagerへの参照を取得
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update内に以下のような変更を加えます：

    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "Main")
        {
            if (gameManager.CurrentPlayer == (int)PieceTeam.Blue)
            {
                // 人間の手番
                HandlePlayerTurn();
            }
            if (gameManager.CurrentPlayer == (int)PieceTeam.Orange)
            {
                // 人間の手番
                HandlePlayerTurn();

            }
        }
        else if (sceneName == "AI")
        {
            if (gameManager.CurrentPlayer == (int)PieceTeam.Blue)
            {
                // 人間の手番
                HandlePlayerTurn();
            }
            if (gameManager.CurrentPlayer == (int)PieceTeam.Orange)
            {
                // コンピュータの手番
                HandleComputerTurn();
            }
        }
    }

    void HandlePlayerTurn()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (selectedPiece == null)
                {

                    if (hit.collider.CompareTag("Player1") && gameManager.CurrentPlayer == (int)PieceTeam.Blue)
                    {
                        // Player1のターンでPlayer1の駒を選択
                        selectedPiece = hit.transform;
                        // 駒のマテリアルを緑色に変更
                        selectedPiece.GetComponent<Renderer>().material = MaterialGreen;
                        audioSource.PlayOneShot(clickSound);

                    }
                    else if (hit.collider.CompareTag("Player2") && gameManager.CurrentPlayer == (int)PieceTeam.Orange)
                    {
                        // Player2のターンでPlayer2の駒を選択
                        selectedPiece = hit.transform;
                        // 駒のマテリアルを緑色に変更
                        selectedPiece.GetComponent<Renderer>().material = MaterialGreen;
                        audioSource.PlayOneShot(clickSound);

                    }
                }
                else
                {

                    // 駒の新しい位置を地面のセルの位置に基づいて設定
                    Vector3 newPiecePosition = hit.collider.transform.position + new Vector3(0, 2, 0); // 2は駒の高さとして仮定

                    // ここから追加機能のコードを組み込む
                    Piece existingPiece = hit.collider.GetComponent<Piece>();

                    // 新しい位置でレイキャストを使用して駒を検出
                    RaycastHit hitInfo;
                    bool movePerformed = false;


                    // 以前のコードのように、特定のセルの名前に基づいて位置を調整する場合
                    if (hit.collider.name == "cube1-1")
                    {
                        newPiecePosition = new Vector3(0, 2, 0);
                    }
                    else if (hit.collider.name == "cube1-2")
                    {
                        newPiecePosition = new Vector3(1.25f, 2, 0);
                    }
                    else if (hit.collider.name == "cube1-3")
                    {
                        newPiecePosition = new Vector3(2.5f, 2, 0);
                    }
                    else if (hit.collider.name == "cube2-1")
                    {
                        newPiecePosition = new Vector3(0, 2, 1.25f);
                    }
                    else if (hit.collider.name == "cube2-2")
                    {
                        newPiecePosition = new Vector3(1.25f, 2, 1.25f);
                    }
                    else if (hit.collider.name == "cube2-3")
                    {
                        newPiecePosition = new Vector3(2.5f, 2, 1.25f);
                    }
                    else if (hit.collider.name == "cube3-1")
                    {
                        newPiecePosition = new Vector3(0, 2, 2.5f);
                    }
                    else if (hit.collider.name == "cube3-2")
                    {
                        newPiecePosition = new Vector3(1.25f, 2, 2.5f);
                    }
                    else if (hit.collider.name == "cube3-3")
                    {
                        newPiecePosition = new Vector3(2.5f, 2, 2.5f);
                    }

                    if (Physics.Raycast(newPiecePosition, Vector3.down, out hitInfo, 2.5f))
                    {
                        if (hitInfo.collider.gameObject != selectedPiece.gameObject)
                        {
                            Strength strengthComponent = hitInfo.collider.GetComponent<Strength>();
                            if (strengthComponent != null && strengthComponent.GetStrength() >= selectedPiece.GetComponent<Strength>().GetStrength())
                            {
                                // 移動不可の場合
                                Debug.Log("移動できません: 目的地の駒が同等かそれ以上の強さです");
                                // 駒のマテリアルを変更
                                if (selectedPiece.CompareTag("Player1"))
                                {
                                    selectedPiece.GetComponent<Renderer>().material = MaterialBlue;
                                }
                                else if (selectedPiece.CompareTag("Player2"))
                                {
                                    selectedPiece.GetComponent<Renderer>().material = MaterialOrange;
                                }
                                audioSource.PlayOneShot(missSound);
                            }
                            else
                            {
                                // 移動が許可された場合、駒の位置を更新
                                selectedPiece.position = newPiecePosition;
                                movePerformed = true;

                            }
                        }                                 
                        else
                        {
                            // 選択された駒以外に駒が検出されなかった場合、移動を実行
                            selectedPiece.position = newPiecePosition;
                            movePerformed = true;
                        }
                    }
                    else
                    {
                        // レイキャストで駒が検出されなかった場合、移動を実行
                        selectedPiece.position = newPiecePosition;
                        movePerformed = true;


                    }
                 
                    if (movePerformed == true)
                    {

                        // 最終的に駒の位置を更新
                        selectedPiece.position = newPiecePosition;

                        // 駒のマテリアルを変更
                        if (selectedPiece.CompareTag("Player1"))
                        {
                            selectedPiece.GetComponent<Renderer>().material = MaterialBlue;
                        }
                        else if (selectedPiece.CompareTag("Player2"))
                        {
                            selectedPiece.GetComponent<Renderer>().material = MaterialOrange;
                        }
                        audioSource.PlayOneShot(getSound);

                        // ターンを切り替える
                        gameManager.SwitchTurn();
                    }

                    // 駒の選択を解除
                    selectedPiece = null;
                }               
            }
        }
    }

    void HandleComputerTurn()
    {
        // コンピュータの手番の処理
        // ゲーム内のすべてのセルを取得するなど、使用可能なセルのリストを用意する必要がある
        // 以下の例では、すべてのセルを直接使用するものと仮定
        GameObject[] allCells = GameObject.FindGameObjectsWithTag("Ground"); // "Ground" タグを持つすべてのセルを取得

        // ランダムにセルを選択
        int randomCellIndex = Random.Range(0, allCells.Length);
        Transform selectedCell = allCells[randomCellIndex].transform;

        // コンピュータが駒を選択して移動するロジックを実装
        if (selectedPiece == null)// 駒の選択
        {
            // 駒の選択ロジック（コンピュータが駒を選択する場合の処理）
            // 以下に、ゲームのルールに合わせて適切な駒選択ロジックを実装
            // 以下は、"Player2" タグを持つ駒をすべて取得し、その中からランダムに駒を選択

            // ランダムに駒を選択
            GameObject[] allPlayer2Pieces = GameObject.FindGameObjectsWithTag("Player2"); // "Player2" タグを持つ駒の配列を取得
            int randomPieceIndex = Random.Range(0, allPlayer2Pieces.Length);
            selectedPiece = allPlayer2Pieces[randomPieceIndex].transform;

            // 駒を選択した時のマテリアルの変更
            selectedPiece.GetComponent<Renderer>().material = MaterialGreen;
        }
        else
        {
            // 駒の移動先を選択したセルに設定
            Vector3 newPiecePosition = selectedCell.position + new Vector3(0, 2, 0); // 2は駒の高さとして仮定

            // 以下のコードで、駒の移動ロジックを実装。
            RaycastHit hitInfo;
            bool movePerformed = false;

            if (Physics.Raycast(newPiecePosition, Vector3.down, out hitInfo, 2.5f))
            {
                if (hitInfo.collider.gameObject != selectedPiece.gameObject)
                {
                    Strength strengthComponent = hitInfo.collider.GetComponent<Strength>();
                    if (strengthComponent != null && strengthComponent.GetStrength() >= selectedPiece.GetComponent<Strength>().GetStrength())
                    {
                        // 移動不可の場合
                        Debug.Log("移動できません: 目的地の駒が同等かそれ以上の強さです");
                    }
                    else
                    {
                        // 移動が許可された場合、駒の位置を更新
                        selectedPiece.position = newPiecePosition;
                        movePerformed = true;
                    }
                }
                else
                {
                    // 選択された駒以外に駒が検出されなかった場合、移動を実行
                    selectedPiece.position = newPiecePosition;
                    movePerformed = true;
                }
            }
            else
            {
                // レイキャストで駒が検出されなかった場合、移動を実行
                selectedPiece.position = newPiecePosition;
                movePerformed = true;
            }

            if (movePerformed == true)
            {

                // 最終的に駒の位置を更新
                selectedPiece.position = newPiecePosition;

                // 駒のマテリアルを変更
                if (selectedPiece.CompareTag("Player1"))
                {
                    selectedPiece.GetComponent<Renderer>().material = MaterialBlue;
                }
                else if (selectedPiece.CompareTag("Player2"))
                {
                    selectedPiece.GetComponent<Renderer>().material = MaterialOrange;
                }
                audioSource.PlayOneShot(getSound);

                // ターンを切り替える
                gameManager.SwitchTurn();
            }

            // 駒の選択を解除
            selectedPiece = null;
        }
    }
}