﻿// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography.X509Certificates;
using ClassLibrary1;
using Runner;

int[,] someMap = new[,] {
    { 0, 5, 1, 1 	,     	 0, 5, 1, 1 	,     	 0, 5, 1, 1 	,     	 0, 5, 1, 1 	},
    { -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	},
    { 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	},
    { 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	},
    
    { 0, 5, 1, 1 	,     	 0, 5, 1, 1 	,     	 0, 5, 1, 1 	,     	 0, 5, 1, 1 	},
    { -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	},
    { 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	},
    { 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	},
    
    { 0, 5, 1, 1 	,     	 0, 5, 1, 1 	,     	 0, 5, 1, 1 	,     	 0, 5, 1, 1 	},
    { -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	},
    { 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	},
    { 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	},
    
    { 0, 5, 1, 1 	,     	 0, 5, 1, 1 	,     	 0, 5, 1, 1 	,     	 0, 5, 1, 1 	},
    { -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	,   	 -1, 5, -1, 1 	},
    { 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	},
    { 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	,     	 2, 5, 1, 1 	},
};

Poc.Run(someMap);