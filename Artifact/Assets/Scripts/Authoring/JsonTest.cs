using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using System.Text;

[System.Serializable]
public class AgentData
{
    public int id { get; set; }
    public List<int> pos { get; set; }

    public Vector3 GetPos()
    {
        return new Vector3(pos[0], 0, pos[1]);
    }
}

public class JsonTest : MonoBehaviour
{
    public Transform agentPrefab;

    public TextMeshProUGUI coordinatesText;

    public List<List<AgentData>> agentsData;

    private Transform agent;
    private int agentIndex;

    private string[] jsonArray = new string[]
    {
    "[{\"id\":1,\"pos\":[99,73]}]",
    "[{\"id\":1,\"pos\":[99,73]}]",
    "[{\"id\":1,\"pos\":[0,74]}]",
    "[{\"id\":1,\"pos\":[1,74]}]",
    "[{\"id\":1,\"pos\":[0,74]}]",
    "[{\"id\":1,\"pos\":[1,73]}]",
    "[{\"id\":1,\"pos\":[1,72]}]",
    "[{\"id\":1,\"pos\":[0,71]}]",
    "[{\"id\":1,\"pos\":[99,71]}]",
    "[{\"id\":1,\"pos\":[98,71]}]",
    "[{\"id\":1,\"pos\":[98,72]}]",
    "[{\"id\":1,\"pos\":[98,73]}]",
    "[{\"id\":1,\"pos\":[98,73]}]",
    "[{\"id\":1,\"pos\":[98,74]}]",
    "[{\"id\":1,\"pos\":[97,74]}]",
    "[{\"id\":1,\"pos\":[98,73]}]",
    "[{\"id\":1,\"pos\":[99,74]}]",
    "[{\"id\":1,\"pos\":[99,75]}]",
    "[{\"id\":1,\"pos\":[99,74]}]",
    "[{\"id\":1,\"pos\":[98,74]}]",
    "[{\"id\":1,\"pos\":[97,73]}]",
    "[{\"id\":1,\"pos\":[98,72]}]",
    "[{\"id\":1,\"pos\":[98,72]}]",
    "[{\"id\":1,\"pos\":[97,73]}]",
    "[{\"id\":1,\"pos\":[96,73]}]",
    "[{\"id\":1,\"pos\":[95,74]}]",
    "[{\"id\":1,\"pos\":[96,73]}]",
    "[{\"id\":1,\"pos\":[97,72]}]",
    "[{\"id\":1,\"pos\":[96,71]}]",
    "[{\"id\":1,\"pos\":[97,71]}]",
    "[{\"id\":1,\"pos\":[97,70]}]",
    "[{\"id\":1,\"pos\":[96,70]}]",
    "[{\"id\":1,\"pos\":[96,70]}]",
    "[{\"id\":1,\"pos\":[95,71]}]",
    "[{\"id\":1,\"pos\":[95,70]}]",
    "[{\"id\":1,\"pos\":[95,71]}]",
    "[{\"id\":1,\"pos\":[94,71]}]",
    "[{\"id\":1,\"pos\":[94,71]}]",
    "[{\"id\":1,\"pos\":[93,70]}]",
    "[{\"id\":1,\"pos\":[92,69]}]",
    "[{\"id\":1,\"pos\":[92,69]}]",
    "[{\"id\":1,\"pos\":[91,70]}]",
    "[{\"id\":1,\"pos\":[92,70]}]",
    "[{\"id\":1,\"pos\":[91,71]}]",
    "[{\"id\":1,\"pos\":[91,71]}]",
    "[{\"id\":1,\"pos\":[92,72]}]",
    "[{\"id\":1,\"pos\":[93,72]}]",
    "[{\"id\":1,\"pos\":[93,72]}]",
    "[{\"id\":1,\"pos\":[94,72]}]",
    "[{\"id\":1,\"pos\":[93,72]}]",
    "[{\"id\":1,\"pos\":[93,73]}]",
    "[{\"id\":1,\"pos\":[94,73]}]",
    "[{\"id\":1,\"pos\":[94,72]}]",
    "[{\"id\":1,\"pos\":[93,73]}]",
    "[{\"id\":1,\"pos\":[94,72]}]",
    "[{\"id\":1,\"pos\":[93,72]}]",
    "[{\"id\":1,\"pos\":[94,72]}]",
    "[{\"id\":1,\"pos\":[93,72]}]",
    "[{\"id\":1,\"pos\":[94,72]}]",
    "[{\"id\":1,\"pos\":[95,73]}]",
    "[{\"id\":1,\"pos\":[94,73]}]",
    "[{\"id\":1,\"pos\":[94,72]}]",
    "[{\"id\":1,\"pos\":[94,73]}]",
    "[{\"id\":1,\"pos\":[95,74]}]",
    "[{\"id\":1,\"pos\":[95,74]}]",
    "[{\"id\":1,\"pos\":[95,73]}]",
    "[{\"id\":1,\"pos\":[94,74]}]",
    "[{\"id\":1,\"pos\":[94,75]}]",
    "[{\"id\":1,\"pos\":[94,76]}]",
    "[{\"id\":1,\"pos\":[93,76]}]",
    "[{\"id\":1,\"pos\":[93,75]}]",
    "[{\"id\":1,\"pos\":[92,75]}]",
    "[{\"id\":1,\"pos\":[92,74]}]",
    "[{\"id\":1,\"pos\":[91,74]}]",
    "[{\"id\":1,\"pos\":[90,74]}]",
    "[{\"id\":1,\"pos\":[90,75]}]",
    "[{\"id\":1,\"pos\":[89,74]}]",
    "[{\"id\":1,\"pos\":[90,73]}]",
    "[{\"id\":1,\"pos\":[89,73]}]",
    "[{\"id\":1,\"pos\":[90,73]}]",
    "[{\"id\":1,\"pos\":[89,74]}]",
    "[{\"id\":1,\"pos\":[90,74]}]",
    "[{\"id\":1,\"pos\":[91,74]}]",
    "[{\"id\":1,\"pos\":[91,75]}]",
    "[{\"id\":1,\"pos\":[91,75]}]",
    "[{\"id\":1,\"pos\":[90,76]}]",
    "[{\"id\":1,\"pos\":[91,76]}]",
    "[{\"id\":1,\"pos\":[91,76]}]",
    "[{\"id\":1,\"pos\":[92,76]}]",
    "[{\"id\":1,\"pos\":[92,76]}]",

    };

    private void Start()
    {
        agentsData = new List<List<AgentData>>(jsonArray.Length);

        for (int i = 0; i < jsonArray.Length; i++)
        {
            agentsData.Add(JsonConvert.DeserializeObject<List<AgentData>>(jsonArray[i]));
        }

        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NextPosition(agentIndex);
        }
    }

    private void Init()
    {
        agent = Instantiate(agentPrefab);
        NextPosition(agentIndex);
    }

    private void NextPosition(int index)
    {
        agent.position = agentsData[index][0].GetPos();
        agentIndex = (agentIndex + 1) % agentsData.Count;

        SetText(agent.position);
        // idToTransform[agentsData[index].id].position = agentsData[index].GetPos();
    }

    private void SetText(Vector3 position)
    {
        coordinatesText.text = $"X: {position.x}, Z: {position.z}";
    }
}