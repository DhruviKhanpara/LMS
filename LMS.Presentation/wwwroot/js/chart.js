const chartInstances = {};

function doughnutChart(chartName, datasetLabel, labels, bgColors, dataList, isDisplayLabels)
{
    var ctx = document.getElementById(chartName).getContext('2d');

    // Destroy previous chart if it exists
    if (chartInstances[chartName])
    {
        chartInstances[chartName].destroy();
    }

    // Check if all values are zero
    const allZero = dataList.every(value => value === 0);

    // If all values are 0, use a placeholder dataset
    const chartData = allZero
        ? {
            labels: ['No Data'],
            datasets: [{
                label: 'No Data',
                data: [1],
                backgroundColor: ['#f2f2f2'],
                borderWidth: 0,
                hoverOffset: 0
            }]
        }
        : {
            labels: labels,
            datasets: [{
                label: datasetLabel,
                data: dataList,
                backgroundColor: bgColors,
                hoverOffset: 1
            }]
        };

    var options = {
        borderRadius: allZero ? 0 : 10,
        offset: allZero ? 0 : 10,
        responsive: true,
        plugins: {
            legend: {
                display: !allZero && isDisplayLabels,
                labels: {
                    color: '#333',
                    font: {
                        size: 14,
                        weight: 'bold'
                    },
                    boxWidth: 15,
                    boxHeight: 15,
                    padding: 10,
                    borderRadius: 50
                }
            },
            tooltip: {
                callbacks: {
                    title: function (context)
                    {
                        return datasetLabel;
                    },
                    label: function (context)
                    {
                        if (allZero)
                        {
                            return context.label;
                        }
                        return `${context.label}: ${context.formattedValue}`;
                    }
                }

            },
            datalabels: {
                display: !allZero,
                formatter: (value, ctx) =>
                {
                    const dataArr = ctx.chart.data.datasets[0].data;
                    const sum = dataArr.reduce((a, b) => a + b, 0);
                    const percentage = ((value / sum) * 100).toFixed(2) + "%";
                    return percentage;
                },
                color: function (context)
                {
                    const bgColor = context.dataset.backgroundColor[context.dataIndex];
                    const rgb = hexToRgb(bgColor);
                    const luminance = 0.299 * rgb.r + 0.587 * rgb.g + 0.114 * rgb.b;
                    return luminance > 140 ? 'black' : 'white';
                },
            },
        }
    };

    // Define the plugin
    const centerTextPlugin = {
        id: 'centerTextPlugin',
        afterDraw(chart)
        {
            if (!allZero) return;

            const { ctx, chartArea: { top, bottom, left, right, width, height } } = chart;

            ctx.save();

            ctx.font = 'bold 20px Arial';
            ctx.fillStyle = '#666';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';

            ctx.fillText('No Data', left + width / 2, top + height / 2);

            ctx.restore();
        }
    };

    chartInstances[chartName] = new Chart(ctx, {
        type: 'doughnut',
        data: chartData,
        options: options,
        plugins: [centerTextPlugin]
    });
}

function lineChart(chartName, datasetLabels, labels, bgColors, dataLists, isDisplayLabels, displayXticks, displayYticks, displayXGrid, displayYGrid)
{
    var ctx = document.getElementById(chartName).getContext('2d');

    // Destroy previous chart if it exists
    if (chartInstances[chartName])
    {
        chartInstances[chartName].destroy();
    }

    const chartData = {
        labels: labels,
        datasets: datasetLabels.map((labelGroup, index) => ({
            label: labelGroup,
            data: dataLists[index],
            backgroundColor: bgColors[index],
            borderColor: bgColors[index],
            tension: 0.4,
            fill: false,
            pointBackgroundColor: bgColors[index]
        }))
    };

    const options = {
        responsive: true,
        plugins: {
            title: {
                display: false
            },
            legend: {
                display: isDisplayLabels,
                labels: {
                    color: '#333',
                    font: {
                        size: 14,
                        weight: 'bold'
                    },
                    boxWidth: 15,
                    boxHeight: 15,
                    padding: 10,
                    borderRadius: 50
                }
            },
            datalabels: {
                display: false,
            }
        },
        scales: {
            x: {
                ticks: { display: displayXticks },
                grid: { display: displayXGrid },
                title: { display: false }
            },
            y: {
                grid: { display: displayYGrid },
                title: { display: false },
                ticks: {
                    display: displayYticks,
                    beginAtZero: true
                }
            }
        }
    };

    chartInstances[chartName] = new Chart(ctx, {
        type: 'line',
        data: chartData,
        options: options
    });
}

function barChart(chartName, datasetLabels, labels, bgColors, dataLists, isDisplayLabels)
{
    var ctx = document.getElementById(chartName).getContext('2d');

    // Destroy previous chart if it exists
    if (chartInstances[chartName])
    {
        chartInstances[chartName].destroy();
    }

    var chartData = {
        labels: labels,
        datasets: datasetLabels.map((labelGroup, index) => ({
            label: labelGroup,
            data: dataLists[index],
            backgroundColor: bgColors[index],
            borderColor: bgColors[index],
            fill: false,
        }))
    };

    var options = {
        borderRadius: 10,
        offset: 20,
        responsive: true,
        plugins: {
            title: {
                display: false
            },
            legend: {
                display: isDisplayLabels,
                labels: {
                    color: '#333',
                    font: {
                        size: 14,
                        weight: 'bold'
                    },
                    boxWidth: 15,
                    boxHeight: 15,
                    padding: 10,
                    borderRadius: 50
                }
            },
            datalabels: {
                display: false,
            }
        }
    };

    chartInstances[chartName] = new Chart(ctx, {
        type: 'bar',
        data: chartData,
        options: options
    });
}

function hexToRgb(hex)
{
    hex = hex.replace('#', '');
    const bigint = parseInt(hex, 16);
    return {
        r: (bigint >> 16) & 255,
        g: (bigint >> 8) & 255,
        b: bigint & 255
    };
}