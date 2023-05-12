
//var myChart = new Chart(document.getElementById("bar-chart"), {
//    type: 'bar',
//    data: {
//        labels: ["Africa", "Asia", "Europe", "Latin America", "North America"],
//        datasets: [
//            {
//                label: "Population (millions)",
//                backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#e8c3b9", "#c45850"],
//                data: [2478, 5267, 734, 784, 433]
//            }
//        ]
//    },
//    options: {
//        responsive: false,
//        legend: { display: false },
//        title: {
//            display: true,
//            text: 'Predicted world population (millions) in 2050'
//        }
//    }
//});




$(document).ready(function () {
	var doughnutChart = new Chart(document.getElementById("doughnut-chart"), {
		type: 'doughnut',
		data: {
			labels: ['Red', 'Orange', 'Yellow', 'Green', 'Blue'],
			datasets: [
				{
					label: 'Dataset 1',
					data: [133,123,43,312,312],
					backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#e8c3b9", "#c45850"],
				}
			]
		},
		options: {
			responsive: true,
			plugins: {
				legend: {
					position: 'left',
				},
				title: {
					display: true,
					text: 'Chart.js Doughnut Chart'
				}
			}
		},
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
					position: 'left',
				},
				title: {
					display: true,
					text: 'Chart.js Pie Chart'
				}
			}
		},
	});

	var polarChart;

	$.ajax({
		type: 'GET',
		url: "?handler=ViolationByAreas&type=" + 'week',
		contentType: false,
		processData: false,
		success: function (res) {
			polarChart = new Chart(document.getElementById("polar-chart"), {
				type: 'polarArea',
				data: {
					labels: res.labels,
					datasets: [
						{
							label: 'Обнаружения',
							data: res.data,
							backgroundColor: ['rgb(255, 99, 132)', 'rgb(255, 159, 64)', 'rgb(255, 205, 86)', 'rgb(75, 192, 192)', 'rgb(153, 102, 255)'],
							borderColor: 'rgb(0, 0, 0, 0.0)',
							pointBackgroundColor: 'rgb(0, 0, 0, 0.0)',
						},
						{
							label: 'Обнаружения',
							data: res.data,
							backgroundColor: ['rgb(255, 99, 132)', 'rgb(255, 159, 64)', 'rgb(255, 205, 86)', 'rgb(75, 192, 192)', 'rgb(153, 102, 255)'],
							borderColor: 'rgb(0, 0, 0, 0.0)',
							pointBackgroundColor: 'rgb(0, 0, 0, 0.0)',
						}
					]
				},
				options: {
					responsive: true,
					scales: {
						r: {
							//pointLabels: {
							//	display: true,
							//	centerPointLabels: true,
							//	font: {
							//		size: 18
							//	}
							//},
							grid: {
								color: "rgb(255, 255, 255, 0.3)",
							},
							ticks: {
								backdropColor: "rgb(0, 0, 0, 0)",
								color: "rgb(255, 255, 255, 0.7)",
							}
						}
					},
					plugins: {
						legend: {
							position: 'left',
							labels: {
								fontColor: 'white'
							}
						},
						title: {
							display: true,
							text: 'Chart.js Radar Chart',
							fontColor: 'white'
						},
					}
				},
			});

		},
		error: function (err) {
			console.log(err)
		}
	})

	var lineChart;

	$.ajax({
		type: 'GET',
		url: "?handler=ViolationCountByDate&type=" + 'minute' + "&fromDate=" + 'LRDateTimeSecond',
		contentType: false,
		processData: false,
		success: function (res) {
			console.log(res);
			labels = res.labels;
			datapoints = res.data;
			lineChart = new Chart(document.getElementById("line-chart"), {
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
						},
						//{
						//	label: 'Cubic interpolation',
						//	data: datapoints,
						//	borderColor: "#006400",
						//	fill: false,
						//	tension: 0.4
						//}, {
						//	label: 'Linear interpolation (default)',
						//	data: datapoints,
						//	borderColor: "#FF7F50",
						//	fill: false
						//}
					]
				},
				options: {
					maintainAspectRatio: false,
					responsive: true,
					elements: {
						point: {
							radius: 0.5
						}
					},
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
							},
							ticks: {
								maxTicksLimit: 10
							}
						},
						y: {
							display: true,
							title: {
								display: true,
								text: 'Value'
							},
							suggestedMin: Math.min(datapoints),
							suggestedMax: Math.max(datapoints),
						}
					}
				},
			})
		},
		error: function (err) {
			console.log(err)
		}
	})

	 

	jQueryCreateLineChart = (type, fieldId) => {
		console.log(document.getElementById(fieldId));
		let from = document.getElementById(fieldId).value;
		let url = "?handler=ViolationCountByDate&type=" + type + "&fromDate="+from;
		console.log(url);
		try {
			$.ajax({
				type: 'GET',
				url: url,
				contentType: false,
				processData: false,
				success: function (res) {
					console.log(res);
					lineChart.data.labels = res.labels;
					lineChart.data.datasets =
						[
							{
								label: 'Cubic interpolation (monotone)',
								data: res.data,
								borderColor: "#BA55D3",
								fill: false,
								cubicInterpolationMode: 'monotone',
								tension: 0.4
						},
						];
					lineChart.options.scales.y.suggestedMin = Math.min(datapoints);
					lineChart.options.scales.y.suggestedMax = Math.max(datapoints),

					lineChart.update();

				},
				error: function (err) {
					console.log(err)
				}
			})
			return false;
		} catch (ex) {
			console.log(ex)
		}
	}

	jQueryCreatePolarChart = (type) => {
		let url = "?handler=ViolationByAreas&type=" + type; 
		console.log(url);
		try {
			$.ajax({
				type: 'GET',
				url: url,
				contentType: false,
				processData: false,
				success: function (res) {
					console.log(res);
					polarChart.data.labels = res.labels;
					polarChart.data.datasets[0].data = res.data;

					polarChart.update();

				},
				error: function (err) {
					console.log(err)
				}
			})
			return false;
		} catch (ex) {
			console.log(ex)
		}
	}





});

function createLineChart(datapoints, labels) {
	console.log(datapoints);
	//labels = [];
	//for (let i = 0; i < 12; ++i) {
	//    labels.push(i.toString());
	//}
	//const datapoints = [0, 20, 20, 60, 60, 120, NaN, 180, 120, 125, 105, 110, 170];
	try {
		lineChart.clear()
	} catch (e) {

	}
	
}
//var hBarChart = new Chart(document.getElementById("horiz-bar-chart"), {
//    type: 'bar',
//    data: {
//        labels: labels,
//        datasets: [
//            {
//                label: 'Dataset 1',
//                data: datapoints,
//                borderColor: "#BA55D3",
//                backgroundColor: "#FF0F1393",
//            },
//            {
//                label: 'Dataset 2',
//                data: datapoints,
//                borderColor: "#006400",
//                backgroundColor: "#0FC5FF93",
//            }
//        ]
//    }
//,
//    options: {
//        indexAxis: 'y',
//        // Elements options apply to all of the options unless overridden in a dataset
//        // In this case, we are setting the border of each horizontal bar to be 2px wide
//        elements: {
//            bar: {
//                borderWidth: 2,
//            }
//        },
//        responsive: true,
//        plugins: {
//            legend: {
//                position: 'right',
//            },
//            title: {
//                display: true,
//                text: 'Chart.js Horizontal Bar Chart'
//            }
//        }
//    },
//})

//const inputs = {
//    min: 8,
//    max: 16,
//    count: 8,
//    decimals: 2,
//    continuity: 1
//};


//const labels = ["1", "2", "3", "4", "5", "6", "7", "8"];

//var radarChart = new Chart(document.getElementById("radar-chart"), {
//    type: 'radar',
//    data: {
//        labels: generateLabels(),
//        datasets: [
//            {
//                label: 'D0',
//                data: [9, 10, 13, 10, 12, 13, 10, 9],
//                borderColor: "red",
//                backgroundColor: "red",
//            },
//            {
//                label: 'D1',
//                data: [9, 10, 13, 10, 12, 13, 10, 9],
//                borderColor: "yellow",
//                hidden: true,
//                backgroundColor: "yellow",
//                fill: '-1'
//            },
//            {
//                label: 'D2',
//                data: [9, 10, 13, 10, 12, 13, 10, 9],
//                borderColor: "blue",
//                backgroundColor: "blue",
//                fill: 1
//            },
//            {
//                label: 'D3',
//                data: [9, 10, 13, 10, 12, 13, 10, 9],
//                borderColor: "green",
//                backgroundColor: "green",
//                fill: false
//            },
//            {
//                label: 'D4',
//                data: [9, 10, 13, 10, 12, 13, 10, 9],
//                borderColor: "black",
//                backgroundColor: "black",
//                fill: '-1'
//            },
//            {
//                label: 'D5',
//                data: [9, 10, 13, 10, 12, 13, 10, 9],
//                borderColor: "purple",
//                backgroundColor: "purple",
//                fill: '-1'
//            },
//            {
//                label: 'D6',
//                data: [9, 10, 13, 10, 12, 13, 10, 9],
//                borderColor: "grey",
//                backgroundColor: "grey",
//                fill: { value: 85 }
//            }
//        ]
//    },
//    options: {
//        plugins: {
//            filler: {
//                propagate: false
//            },
//            'samples-filler-analyser': {
//                target: 'chart-analyser'
//            }
//        },
//        interaction: {
//            intersect: false
//        }
//    }
//})