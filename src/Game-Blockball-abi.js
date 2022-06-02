module.exports = [
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "blockheads",
				"type": "address"
			}
		],
		"stateMutability": "nonpayable",
		"type": "constructor"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "string",
				"name": "action",
				"type": "string"
			}
		],
		"name": "Ball",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "string",
				"name": "s",
				"type": "string"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "x",
				"type": "uint256"
			}
		],
		"name": "I",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "bytes",
				"name": "b",
				"type": "bytes"
			}
		],
		"name": "LogBytes",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "bytes32",
				"name": "b",
				"type": "bytes32"
			}
		],
		"name": "LogBytes32",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "userB",
				"type": "address"
			},
			{
				"indexed": true,
				"internalType": "address",
				"name": "userR",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "scoreB",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "scoreR",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint32[8]",
				"name": "players",
				"type": "uint32[8]"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "blocktrophyIndex",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "blockletIndex",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "bytes32",
				"name": "randomSeed",
				"type": "bytes32"
			}
		],
		"name": "Match",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p0",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p1",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p2",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p3",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p4",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p5",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p6",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p7",
				"type": "uint256"
			}
		],
		"name": "Positions",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "string",
				"name": "s",
				"type": "string"
			}
		],
		"name": "Section",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "r",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "s0",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "s1",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "c",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p0",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p1",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p2",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p3",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p4",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p5",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p6",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "p7",
				"type": "uint256"
			}
		],
		"name": "State",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "dp",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "dr",
				"type": "uint256"
			}
		],
		"name": "StateExcess",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "tournamentId",
				"type": "uint256"
			}
		],
		"name": "Tournament",
		"type": "event"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "x",
				"type": "uint256"
			}
		],
		"name": "addPlayer",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint32",
				"name": "p1",
				"type": "uint32"
			},
			{
				"internalType": "uint32",
				"name": "p2",
				"type": "uint32"
			},
			{
				"internalType": "uint32",
				"name": "p3",
				"type": "uint32"
			},
			{
				"internalType": "uint32",
				"name": "p4",
				"type": "uint32"
			}
		],
		"name": "addTeam",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "awardPrizes",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "blockballContractAddress",
		"outputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "blockheadsContractAddress",
		"outputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "blockletsContractAddress",
		"outputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "blockmintingContractAddress",
		"outputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "blocktrophiesContractAddress",
		"outputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "blockheadId",
				"type": "uint256"
			},
			{
				"internalType": "bytes32",
				"name": "commitmentHash",
				"type": "bytes32"
			}
		],
		"name": "commitTeams",
		"outputs": [],
		"stateMutability": "payable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "t1",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "t2",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "s",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "rounds",
				"type": "uint256"
			}
		],
		"name": "directPlay",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "result",
				"type": "uint256"
			}
		],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "rounds",
				"type": "uint256"
			},
			{
				"internalType": "bytes32",
				"name": "seed",
				"type": "bytes32"
			},
			{
				"internalType": "uint32[8]",
				"name": "ps",
				"type": "uint32[8]"
			},
			{
				"internalType": "bool",
				"name": "allStates",
				"type": "bool"
			}
		],
		"name": "play",
		"outputs": [
			{
				"internalType": "uint256[]",
				"name": "states",
				"type": "uint256[]"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "playerCount",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "revealsLimit",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "stepsLimit",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "matchesLimit",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "gasRefundPriceMin",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "individualId",
				"type": "uint256"
			}
		],
		"name": "processTournament",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "userAddress",
				"type": "address"
			},
			{
				"internalType": "uint32[]",
				"name": "players",
				"type": "uint32[]"
			},
			{
				"internalType": "bytes32",
				"name": "salt",
				"type": "bytes32"
			}
		],
		"name": "revealTeams",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "blockball",
				"type": "address"
			},
			{
				"internalType": "address",
				"name": "blockminting",
				"type": "address"
			},
			{
				"internalType": "address",
				"name": "blocklets",
				"type": "address"
			},
			{
				"internalType": "address",
				"name": "blocktrophies",
				"type": "address"
			},
			{
				"internalType": "address",
				"name": "utilityFormatting",
				"type": "address"
			}
		],
		"name": "setContractAddresses",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "newOwner",
				"type": "address"
			}
		],
		"name": "setOwner",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "cost",
				"type": "uint256"
			}
		],
		"name": "setStrategyTrainingCost",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "teamCount",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "blockId",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "strategy",
				"type": "uint256"
			}
		],
		"name": "trainStrategy",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "utilityFormattingContractAddress",
		"outputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"stateMutability": "view",
		"type": "function"
	}
]