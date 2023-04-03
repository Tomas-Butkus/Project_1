using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject crackedFloor;
    [SerializeField] [Range(1, 60)] private int destroyCooldown = 10;
    [SerializeField] [Range(1, 100)] private int damageAmount;
    private float nextDestroy;

    private PathfindingGrid pathfindingGrid;
    private AdventurerBehaviour adventurerBehaviour;

    [SerializeField] Tilemap walkableTileMap;
    public AudioSource audioSource;
    public AudioClip breakFloorSound;
    public float volume = 0.5f;

    private void Start()
    {
        pathfindingGrid = FindObjectOfType<PathfindingGrid>();
        adventurerBehaviour = FindObjectOfType<AdventurerBehaviour>();
    }

    private void Update()
    {
        CheckDestructionInput();
        CheckIfGameWasRestarted();
    }

    private void CheckDestructionInput()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > nextDestroy)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tpos = walkableTileMap.WorldToCell(worldPoint);
            TileBase tile = walkableTileMap.GetTile(tpos);

            if(tile)
            {
                audioSource.PlayOneShot(breakFloorSound, volume);

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                PathfindingNode node = pathfindingGrid.NodeFromWorldPoint(mousePos);
                Vector2 mouseWorldPos = node.worldPosition;

                GameObject floor = Instantiate(crackedFloor);
                floor.transform.position = mouseWorldPos;
                adventurerBehaviour.crackedFloors.Add(floor);

                nextDestroy = Time.time + destroyCooldown;
                Damage();
            }
        }
    }

    private void CheckIfGameWasRestarted()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Damage()
    {
        adventurerBehaviour.repair = Mathf.Clamp(adventurerBehaviour.repair - damageAmount, 0, 100);
        adventurerBehaviour.repairBar.SetFill(adventurerBehaviour.repair);
    }

    public void GoToMenu()
    {
        Debug.Log("Going back to main menu!");
        SceneManager.LoadScene(0);
    }
}
