const wrap = d3.select('#chart-wrap');
let wrapWidth = parseInt(wrap.style('width'));
let wrapHeight;
const src = 'https://assets.codepen.io/1329727/data-demo.csv';
const colorsArray = ['#5626C4', '#E60576', '#2CCCC3', '#FACD3D', '#FF5100', '#181818',];
const formatPerc = d3.format(".0%");
let width;
let height;
let pieRadius;
let pie;
let total;
let arc;

// Tooltip functions
const tooltipMouseMove = (d) => {
  const xArc = arc.centroid(d)[0] + (width / 2);
  const yArc = arc.centroid(d)[1] + (height / 2);

  tooltipChart
    .html(() => {
      return (
        `<div class="chart-tooltip-wrap">
            <p>Percentage: ${formatPerc(d.value / total)}</p>
            <p>Value: ${d.value}</p>
           </div>
          `
      );
    })
    .style('visibility', 'visible')
    .style('top', 0)
    .style('left', 0)
    .style("transform", `translate(${xArc + 80}px, ${yArc - 25}px)`);
}

const tooltipMouseOut = () => {
  tooltipChart
    .style('visibility', 'hidden')
}

// Pie
if (wrapWidth <= 479) {
  width = 250;
  height = 250;
} else {
  width = 300;
  height = 300;
}

pieRadius = Math.min(width / 2, height / 2);

pie = d3.pie()
  .value((d) => { return d.value; });

arc = d3.arc()
  .outerRadius(pieRadius)
  .innerRadius(0);

// Animation function
const animate = (f) => {
  f.innerRadius = 0;
  let interp = d3.interpolate({ startAngle: 0, endAngle: 0 }, f);

  return (t) => { return arc(interp(t)); }
}

// SVG
const svg = wrap.append('svg')
  .attr('width', width)
  .attr('height', height);

// SVG aria tags
svg.append('title')
  .attr('id', 'chart-title')
  .html('Pie chart');

svg.append('desc')
  .attr('id', 'chart-desc')
  .html('Displays arbitrary values for items.');

svg.attr('aria-labelledby', 'chart-title chart-desc');

tooltipChart = wrap.append('div')
  .attr('class', 'chart-tooltip')
  .style('visibility', 'hidden');

const g = svg.append('g')
  .attr('width', width)
  .attr('height', height)
  .style('transform', `translate(${width / 2}px, ${height / 2}px)`);

// Legend wrap
const legend = wrap.append('div')
  .attr('class', 'legend');

// Create chart
async function createPie() {
  const data = await d3.csv(src);
  data.sort((a, b) => {
    return d3.descending(+a.value, +b.value);
  });
  total = d3.sum(data, (d) => { return d.value; });

  // Pie slices
  const slices = g.selectAll('.arc')
    .data(pie(data))
    .enter()
    .append('path')
    .attr('class', 'slices')
    .attr('fill', (d, i) => {
      return colorsArray[i];
    })
    .attr('aria-label', d => {
      return `Pie slice for ${d.name}`
    })
    .on('mouseover', (event, d) => {
      const f = d;
      tooltipMouseMove(f);
    })
    .on('mouseout', () => {
      tooltipMouseOut();
    })
    .transition()
    .duration(1000)
    .delay(100)
    .attrTween('d', animate);

  // Text values in slices
  const text = g.selectAll('.text')
    .data(pie(data))
    .enter()
    .append('text')
    .attr('transform', (d) => { return `translate(${arc.centroid(d)})`; })
    .attr('class', 'text')
    .style('opacity', '0')
    .text((d, i) => { return formatPerc(d.value / total) })
    .transition()
    .duration(300)
    .delay(700)
    .style('opacity', '1');

  // Legend
  legend.selectAll('div')
    .data(data)
    .enter()
    .append('div')
    .attr('class', 'legend-group')
    .html((d, i) => {
      return (`
        <div class="legend-box" style="background-color: ${colorsArray[i]};"></div>
        <p class="legend-label">${d.name}</p>
      `)
    });
}
createPie();

const resize = () => {
  let wrapWidth = parseInt(wrap.style('width'));
  if (wrapWidth <= 479) {
    width = 250;
    height = 250;
  } else {
    width = 300;
    height = 300;
  }

  svg.attr('width', width)
    .attr('height', height);

  g.attr('width', width)
    .attr('height', height)
    .style('transform', `translate(${width / 2}px, ${height / 2}px)`);

  pieRadius = Math.min(width / 2, height / 2);

  arc = d3.arc()
    .outerRadius(pieRadius)
    .innerRadius(0);

  d3.selectAll('.slices')
    .attr('d', arc)
    .on('mouseover', (event, d) => {
      const f = d;
      tooltipMouseMove(f);
    })
    .on('mouseout', () => {
      tooltipMouseOut();
    })

  d3.selectAll('.text')
    .attr('transform', (d) => { return `translate(${arc.centroid(d)})`; })
}

d3.select(window).on('resize', resize);