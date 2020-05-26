using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Graphs;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;

/// <summary>
/// This example spawns various theme override volumes around rooms 
/// This is done by hooking into the build events of DA and adding 
/// volumes right after the layout is built, but before the theme engine executes
/// </summary>
public class DansInteriorDesignInspirationStationVolumeSpawnListener : DungeonEventListener {
    // DungeonArchitect.Graphs.Graph is a theme graph asset stored in disk
    // not currently implemented - see OnPostDungeonLayoutBuild()
    public Graph endRoomTheme;
    public Graph spawnRoomTheme;

    // bathroom, office, kitchen, bedroom, dining room, living room
    [Tooltip("Room size - smallest to largest")]
    public Graph[] roomThemes;

    [SerializeField]
    List<GameObject> managedVolumes = new List<GameObject>();

    /// <summary>
    /// The template required to clone and duplicate a theme override volume. 
    /// Supply the reference of the theme override volume prefab here
    /// </summary>
    public Volume themeVolumeTemplate;

    // cell Id, cell area
    private List<int> cellIdList;
    private List<int> cellAreaList;

    // cellId, roomTheme index
    private Dictionary<int,int> cellIdToThemeDictionary;

    /// <summary>
    /// Called after the layout is built in memory, but before the markers are emitted
    /// We would like to spawn volumes here that encompass the rooms, so each room has a different theme applied to it
    /// </summary>
    /// <param name="model">The dungeon model</param>
    public override void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model) {
        DestroyManagedVolumes();

        // Make sure we are working with the grid builder
        var gridModel = model as GridDungeonModel;
        if (gridModel == null) return;

        // Pick the start / end rooms for special decoration
        Cell spawnCell, finalBossCell;
        FindStartEndRooms(gridModel, out spawnCell, out finalBossCell);

        SortCellIdList(gridModel);

        CalculateTotalNumberOfThemes(gridModel);

        // cellId, roomTheme index
        foreach(KeyValuePair<int,int> cell in cellIdToThemeDictionary) {
            DecorateRoom(dungeon, gridModel, gridModel.GetCell(cell.Key), roomThemes[cell.Value]);
        }

        // figure out how to handle spawning start and end rooms

        /// Initial/legacy code
        // Start decorating the rooms with random themes (except start / end rooms which have their own decorations)
        /*foreach (var cell in gridModel.Cells)
        {
            if (cell.CellType != CellType.Room)
            {
                // We only way to decorate rooms
                continue;
            }

            if (cell == spawnCell)
            {
                DecorateRoom(dungeon, gridModel, cell, spawnRoomTheme);
            }
            else if (cell == finalBossCell)
            {
                DecorateRoom(dungeon, gridModel, cell, endRoomTheme);
            }
            else
            {
                DecorateRoom(dungeon, gridModel, cell, GetRandomTheme());
            }
        }*/
    }

    public override void OnDungeonDestroyed(Dungeon dungeon) {
        DestroyManagedVolumes();
    }

    // specific to uniform distribution
    private void CalculateTotalNumberOfThemes(GridDungeonModel gridModel) {
        // find the total number of rooms in the grid
        int numberOfRooms = 0;
        foreach (var cell in gridModel.Cells) {
            if (cell.CellType != CellType.Room) {
                continue;
            }
            numberOfRooms++;
        }

        int counter = 0;
        // create a dictionary with the cellId and theme to be selected
        cellIdToThemeDictionary = new Dictionary<int, int>();
        int numberOfRoomsPerTheme = (int)Mathf.Ceil(numberOfRooms / roomThemes.Length);
        int cellIdCounter = 0;
        for(int i = 0; i < roomThemes.Length; i++) {
            for (int j = 0; j < numberOfRoomsPerTheme; j++) {
                if (cellIdCounter < cellIdList.Count) {
                    cellIdToThemeDictionary.Add(cellIdList[cellIdCounter++], i);
                    counter++;
                }
            }
        }
        Debug.Log("num times callIdToThemeDictionary was added = " + counter);
        Debug.Log("numberOfRooms = " + numberOfRooms + " cellIdList count = " + cellIdList.Count);
    }

    private void SortCellIdList(GridDungeonModel gridModel) {
        cellIdList = new List<int>();
        cellAreaList = new List<int>();

        // determines size of cells, and stores it in the dictionary
        foreach (var cell in gridModel.Cells) {
            if (cell.CellType == CellType.Room) {
                int area = cell.Bounds.Size.x * cell.Bounds.Size.z;
                cellIdList.Add(cell.Id);
                cellAreaList.Add(area);
                //Debug.Log("cell id = " + cell.Id + " cell square foot = " + area);
            }
        }

        int tempArea;
        int tempId;
        // sort dictionary based on size
        for (int j = 0; j <= cellAreaList.Count - 2; j++) {
            for (int i = 0; i <= cellAreaList.Count - 2; i++) {
                if (cellAreaList[i] > cellAreaList[i + 1]) {
                    // sort the area list
                    tempArea = cellAreaList[i + 1];
                    cellAreaList[i + 1] = cellAreaList[i];
                    cellAreaList[i] = tempArea;

                    // sort the id list
                    tempId = cellIdList[i + 1];
                    cellIdList[i + 1] = cellIdList[i];
                    cellIdList[i] = tempId;
                }
            }
        }
    }
    
    void DecorateRoom(Dungeon dungeon, GridDungeonModel gridModel, Cell cell, Graph theme)
    {
        if (theme == null || cell == null) return;

        // Grid size used to convert logical grid coords to world coords
        var gridSize = gridModel.Config.GridCellSize;

        Vector3 position = cell.Bounds.Location * gridSize;
        Vector3 size = cell.Bounds.Size * gridSize;
        var center = position + size / 2.0f;
        var scale = size;
        scale.y = 5;    // Fixed height of the volume.  Optionally make this customizable

        var volumeObject = Instantiate(themeVolumeTemplate.gameObject) as GameObject;
        volumeObject.transform.position = center;
        volumeObject.transform.localScale = scale;
        var volume = volumeObject.GetComponent<ThemeOverrideVolume>();
        volume.dungeon = dungeon;       // Let the volume know that it belongs to this dungeon
        volume.overrideTheme = theme;   // Assign the theme we'd like this volume to override

        // Save a reference to the volume so we can destroy it when it is rebuilt the next time (or we will end up with duplicate volumes on rebuilds)
        managedVolumes.Add(volumeObject);
    }

    Graph GetRandomTheme()
    {
        if (roomThemes.Length == 0)
        {
            return null;
        }
        // Pick a random theme from the supplied theme list
        return roomThemes[Random.Range(0, roomThemes.Length)];
    }

    void FindStartEndRooms(GridDungeonModel gridModel, out Cell spawnCell, out Cell finalBossCell)
    {
        var furthestCells = GridDungeonModelUtils.FindFurthestRooms(gridModel);
        if (furthestCells.Length == 2 && furthestCells[0] != null && furthestCells[1] != null)
        {
            spawnCell = furthestCells[0];
            finalBossCell = furthestCells[1];
        }
        else
        {
            spawnCell = null;
            finalBossCell = null;
        }
    }

    void DestroyManagedVolumes()
    {
        foreach (var volume in managedVolumes)
        {
            if (Application.isPlaying)
            {
                Destroy(volume);
            }
            else
            {
                DestroyImmediate(volume);
            }
        }

        managedVolumes.Clear();
    }

}
