var myChart = new Chart(document.getElementById("bar-chart"), {
    type: 'bar',
    data: {
        labels: ["Africa", "Asia", "Europe", "Latin America", "North America"],
        datasets: [
            {
                label: "Population (millions)",
                backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#e8c3b9", "#c45850"],
                data: [2478, 5267, 734, 784, 433]
            }
        ]
    },
    options: {
        responsive: false,
        legend: { display: false },
        title: {
            display: true,
            text: 'Predicted world population (millions) in 2050'
        }
    }
});


var pieChart = new Chart(document.getElementById("pie-chart"), {
    type: 'pie',
    data: {
        labels: ['Red', 'Orange', 'Yellow', 'Green', 'Blue'],
        datasets: [
            {
                label: 'Dataset 1',
                data: [2478, 5267, 734, 784, 433],
                backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#e8c3b9", "#c45850"],
            }
        ]
    },
    options: {
        responsive: true,
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'Chart.js Pie Chart'
            }
        }
    },
});

const labels = [];
for (let i = 0; i < 12; ++i) {
    labels.push(i.toString());
}
const datapoints = [0, 20, 20, 60, 60, 120, NaN, 180, 120, 125, 105, 110, 170];
var lineChart = new Chart(document.getElementById("line-chart"), {
    type: 'line',
    data: {
        labels: labels,
        datasets: [
            {
                label: 'Cubic interpolation (monotone)',
                data: datapoints,
                borderColor: "#BA55D3",
                fill: false,
                cubicInterpolationMode: 'monotone',
                tension: 0.4
            }, {
                label: 'Cubic interpolation',
                data: datapoints,
                borderColor: "#006400",
                fill: false,
                tension: 0.4
            }, {
                label: 'Linear interpolation (default)',
                data: datapoints,
                borderColor: "#FF7F50",
                fill: false
            }
        ]
    },
    options: {
        responsive: true,
        plugins: {
            title: {
                display: true,
                text: 'Chart.js Line Chart - Cubic interpolation mode'
            },
        },
        interaction: {
            intersect: false,
        },
        scales: {
            x: {
                display: true,
                title: {
                    display: true
                }
            },
            y: {
                display: true,
                title: {
                    display: true,
                    text: 'Value'
                },
                suggestedMin: -10,
                suggestedMax: 200
            }
        }
    },
})

var hBarChart = new Chart(document.getElementById("horiz-bar-chart"), {
    type: 'bar',
    data: {
        labels: labels,
        datasets: [
            {
                label: 'Dataset 1',
                data: datapoints,
                borderColor: "#BA55D3",
                backgroundColor: "#FF0F1393",
            },
            {
                label: 'Dataset 2',
                data: datapoints,
                borderColor: "#006400",
                backgroundColor: "#0FC5FF93",
            }
        ]
    }
,
    options: {
        indexAxis: 'y',
        // Elements options apply to all of the options unless overridden in a dataset
        // In this case, we are setting the border of each horizontal bar to be 2px wide
        elements: {
            bar: {
                borderWidth: 2,
            }
        },
        responsive: true,
        plugins: {
            legend: {
                position: 'right',
            },
            title: {
                display: true,
                text: 'Chart.js Horizontal Bar Chart'
            }
        }
    },
})

const inputs = {
    min: 8,
    max: 16,
    count: 8,
    decimals: 2,
    continuity: 1
};


const labels = ["1", "2", "3", "4", "5", "6", "7", "8"];

var radarChart = new Chart(document.getElementById("radar-chart"), {
    type: 'radar',
    data: {
        labels: generateLabels(),
        datasets: [
            {
                label: 'D0',
                data: [9, 10, 13, 10, 12, 13, 10, 9],
                borderColor: "red",
                backgroundColor: "red",
            },
            {
                label: 'D1',
                data: [9, 10, 13, 10, 12, 13, 10, 9],
                borderColor: "yellow",
                hidden: true,
                backgroundColor: "yellow",
                fill: '-1'
            },
            {
                label: 'D2',
                data: [9, 10, 13, 10, 12, 13, 10, 9],
                borderColor: "blue",
                backgroundColor: "blue",
                fill: 1
            },
            {
                label: 'D3',
                data: [9, 10, 13, 10, 12, 13, 10, 9],
                borderColor: "green",
                backgroundColor: "green",
                fill: false
            },
            {
                label: 'D4',
                data: [9, 10, 13, 10, 12, 13, 10, 9],
                borderColor: "black",
                backgroundColor: "black",
                fill: '-1'
            },
            {
                label: 'D5',
                data: [9, 10, 13, 10, 12, 13, 10, 9],
                borderColor: "purple",
                backgroundColor: "purple",
                fill: '-1'
            },
            {
                label: 'D6',
                data: [9, 10, 13, 10, 12, 13, 10, 9],
                borderColor: "grey",
                backgroundColor: "grey",
                fill: { value: 85 }
            }
        ]
    },
    options: {
        plugins: {
            filler: {
                propagate: false
            },
            'samples-filler-analyser': {
                target: 'chart-analyser'
            }
        },
        interaction: {
            intersect: false
        }
    }
})