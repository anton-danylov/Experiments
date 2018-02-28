function createRow(first, row) {
    let trNode = document.createElement("tr");

    let firstTdNode = document.createElement("td");
    firstTdNode.appendChild(document.createTextNode(first));
    trNode.appendChild(firstTdNode);

    for (var i = 0; i < row.length; i++) {
        let tdNode = document.createElement("td");
        tdNode.appendChild(document.createTextNode(row[i]));
        trNode.appendChild(tdNode);
    }

    return trNode;
}


function printNodes(nodes, offset) {
    for (var i = 0; i < nodes.length; i++) {
        var node = nodes[i];
        document.getElementById("nodes")
            .appendChild(createRow(offset + node.element + "=>", node.slice));

        if (node.children.length > 0) {
            printNodes(node.children, "__ " + offset);
        }
    }
}

function getLowerNodes(arr) {

    var lowerNodes = [];
    for (var i = 0; i < arr.length; i++) {
        let node = { element: arr[i], slice: arr.slice(), children: [] };
        node.slice.splice(i, 1);

        if (node.slice.length > 2) {
            node.children = getLowerNodes(node.slice);
        }

        lowerNodes.push(node);
    }

    return lowerNodes;
}

function findCombinations(node, combinations, currentCombo) {
    currentCombo.push(node.element);
    if (node.slice.length == 1) {
        currentCombo.push(node.slice[0]);
        combinations.push(currentCombo);
    } 
    else if (node.slice.length == 2) {
        let firstCombo = currentCombo.slice();
        let secondCombo = currentCombo.slice();

        firstCombo.push(node.slice[0]);
        firstCombo.push(node.slice[1]);

        secondCombo.push(node.slice[1]);
        secondCombo.push(node.slice[0]);

        combinations.push(firstCombo);
        combinations.push(secondCombo);
    }
    else {
        for (let i = 0; i < node.children.length; i++) {
            let child = node.children[i];
            let childCombo = currentCombo.slice();
            findCombinations(child, combinations, childCombo);
        }
    }
}

function printCombinations() {
    var row = ["1", "2", "3", "4", "5", "6"];
    var nodes = getLowerNodes(row);

    printNodes(nodes, "");
    let combinations = [];
    for (var i = 0; i < nodes.length; i++) {
        let node = nodes[i];

        let nodeCombinations = [];
        findCombinations(node, nodeCombinations, []);
        combinations = combinations.concat(nodeCombinations);
    }


    for (var i = 0; i < combinations.length; i++) {
        var c = combinations[i];
        document.getElementById("permutations")
            .appendChild(createRow("", c));
    }

    console.log(`Total permutations: ${combinations.length}`);
}