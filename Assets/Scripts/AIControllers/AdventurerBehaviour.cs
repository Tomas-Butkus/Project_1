using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdventurerBehaviour : BTAgent
{
    [Header("Needs:")]
    public int energy = 90;
    public int oil = 80;
    public int repair = 75;
    public int repairAmount;
    [SerializeField] private float needDecreaseDelay = 5f;
    public NeedBar energyBar;
    public NeedBar oilBar;
    public NeedBar repairBar;

    private GameObject pickup;

    [Header("Objects in Scene:")]
    public GameObject energyCharger;
    public GameObject oilBarrel;
    public GameObject oilTank;
    public GameObject[] energyCells;
    public GameObject[] oilBarrels;
    public List<GameObject> waypoints;
    public List<GameObject> crackedFloors;

    public override void Start()
    {
        base.Start();
        SetNeedValues();

        // RANDOMIZE ENERGY CELL SELECTION
        RSelector selectEnergyCell = new RSelector("Select energy cell to pick up");
        for (int i = 0; i < energyCells.Length; i++)
        {
            Leaf goToEnergyCell = new Leaf("Go to " + energyCells[i].name, i, GoToEnergyCell);
            selectEnergyCell.AddChild(goToEnergyCell);
        }

        // RANDOMIZE OIL BARREL SELECTION
        RSelector selectOilBarrel = new RSelector("Select oil barrel to pick up");
        for (int i = 0; i < oilBarrels.Length; i++)
        {
            Leaf goToOilBarrel = new Leaf("Go to " + oilBarrels[i].name, i, GoToOilBarrel);
            selectOilBarrel.AddChild(goToOilBarrel);
        }

        // RANDOMIZE WAYPOINTS
        RSelector selectWaypoint = new RSelector("Select a random waypoint");
        for (int i = 0; i < waypoints.Count; i++)
        {
            Leaf goToWaypoint = new Leaf("Go to " + waypoints[i].name, i, GoToWaypoint);
            selectWaypoint.AddChild(goToWaypoint);
        }

        // HAS ENERGY?
        Leaf hasGotEnergy = new Leaf("Has Got Energy", HasEnergy);
        Inverter hasEnergy = new Inverter("Has energy?");
        hasEnergy.AddChild(hasGotEnergy);

        // HAS OIL?
        Leaf hasGotOil = new Leaf("Has Got Oil", HasOil);
        Inverter hasOil = new Inverter("Has oil?");
        hasOil.AddChild(hasGotOil);

        // NEEDS REPAIR?
        Leaf needsRepair = new Leaf("Needs repair", NeedsRepair);
        Inverter needRepair = new Inverter("Need repair?");
        needRepair.AddChild(needsRepair);

        // ADD ENERGY
        Leaf goToEnergyPanel = new Leaf(name + " Go to energy charger", GoToEnergyPanel);

        Sequence addEnergy = new Sequence("Add energy");
        addEnergy.AddChild(hasEnergy);
        addEnergy.AddChild(selectEnergyCell);
        addEnergy.AddChild(goToEnergyPanel);

        // ADD OIL
        Leaf goToOilTank = new Leaf(name + " Go to oil tank", GoToOilTank);

        Sequence fillOil = new Sequence("Fill oil");
        fillOil.AddChild(hasOil);
        fillOil.AddChild(selectOilBarrel);
        fillOil.AddChild(goToOilTank);

        // REPAIR
        Leaf goToCrackedFloor = new Leaf(name + " Go to cracked floor", GoToCrackedFloor);
        Leaf repair = new Leaf(name + " Repair", Repair);

        Sequence performRepair = new Sequence("Perform repair");
        performRepair.AddChild(needRepair);
        performRepair.AddChild(goToCrackedFloor);
        performRepair.AddChild(repair);
        
        // SELECTOR
        Selector selectChore = new Selector("Select chore");
        selectChore.AddChild(addEnergy);
        selectChore.AddChild(fillOil);
        selectChore.AddChild(performRepair);
        //selectChore.AddChild(selectWaypoint);

        // TREE ROOT
        tree.AddChild(selectChore);

        tree.PrintTree();
        StartCoroutine("DecreaseNeeds");
    }

    private void Update()
    {
        CheckNeeds();
    }

    public BTNode.Status HasEnergy()
    {
        if (energy < 70)
        {
            return BTNode.Status.FAILURE;
        }
        return BTNode.Status.SUCCESS;
    }

    public BTNode.Status HasOil()
    {
        if (oil < 75)
        {
            return BTNode.Status.FAILURE;
        }
        return BTNode.Status.SUCCESS;
    }

    public BTNode.Status NeedsRepair()
    {
        if (repair < 80)
        {
            return BTNode.Status.FAILURE;
        }
        return BTNode.Status.SUCCESS;
    }

    public BTNode.Status GoToEnergyCell(int i)
    {
        if (!energyCells[i].activeSelf) return BTNode.Status.FAILURE;
        Vector3 target = pathfindingGrid.NodeFromWorldPoint(energyCells[i].transform.position).worldPosition;
        BTNode.Status s = GoToLocation(target);
        if (s == BTNode.Status.SUCCESS)
        {
            energyCells[i].transform.position = transform.position + new Vector3(0.4f, 0.4f);
            energyCells[i].transform.parent = gameObject.transform;
            pickup = energyCells[i];
        }
        return s;
    }

    public BTNode.Status GoToEnergyPanel()
    {
        Vector3 target = pathfindingGrid.NodeFromWorldPoint(energyCharger.transform.position).worldPosition;
        BTNode.Status s = GoToLocation(target);
        if (s == BTNode.Status.SUCCESS)
        {
            if (pickup != null)
            {
                energy += 30;
                pickup.SetActive(false);
                pickup = null;
            }
        }
        return s;
    }

    public BTNode.Status GoToOilBarrel(int i)
    {
        if (!oilBarrels[i].activeSelf) return BTNode.Status.FAILURE;
        Vector3 target = pathfindingGrid.NodeFromWorldPoint(oilBarrels[i].transform.position).worldPosition;
        BTNode.Status s = GoToLocation(target);
        if (s == BTNode.Status.SUCCESS)
        {
            oilBarrels[i].transform.position = transform.position + new Vector3(0.4f, 0.4f);
            oilBarrels[i].transform.parent = gameObject.transform;
            pickup = oilBarrels[i];
        }
        return s;
    }

    public BTNode.Status GoToOilTank()
    {
        Vector3 target = pathfindingGrid.NodeFromWorldPoint(oilTank.transform.position).worldPosition;
        BTNode.Status s = GoToLocation(target);
        if (s == BTNode.Status.SUCCESS)
        {
            if (pickup != null)
            {
                oil += 25;
                pickup.SetActive(false);
                pickup = null;
            }
        }
        return s;
    }

    public BTNode.Status GoToCrackedFloor()
    {
        if (crackedFloors.Count == 0) return BTNode.Status.FAILURE;
        Vector3 target = pathfindingGrid.NodeFromWorldPoint(crackedFloors[0].transform.position).worldPosition;
        BTNode.Status s = GoToLocation(target);
        return s;
    }
    //
    public BTNode.Status Repair()
    {
        if (crackedFloors.Count == 0) return BTNode.Status.FAILURE;

        GameObject fixedFloor = crackedFloors[0];
        crackedFloors.Remove(fixedFloor);
        Destroy(fixedFloor);

        repair = Mathf.Clamp(repair + repairAmount, 0, 100);
        repairBar.SetFill(repair);

        BTNode.Status s = PerformRepair();
        return s;
    }

    public BTNode.Status GoToWaypoint(int i)
    {
        if (!waypoints[i].activeSelf) return BTNode.Status.FAILURE;
        Vector3 target = pathfindingGrid.NodeFromWorldPoint(waypoints[i].transform.position).worldPosition;
        BTNode.Status s = GoToLocation(target);
        return s;
    }

    private void SetNeedValues()
    {
        energyBar.SetFill(energy);
        oilBar.SetFill(oil);
        repairBar.SetFill(repair);
    }

    private IEnumerator DecreaseNeeds()
    {
        while (true)
        {
            energy = Mathf.Clamp(energy - 3, 0, 100);
            energyBar.SetFill(energy);

            oil = Mathf.Clamp(oil - 2, 0, 100);
            oilBar.SetFill(oil);

            yield return new WaitForSeconds(needDecreaseDelay);
        }
    }

    private void CheckNeeds()
    {
        if(energy <= 0 || oil <= 0 || repair <= 0)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
    }
}
