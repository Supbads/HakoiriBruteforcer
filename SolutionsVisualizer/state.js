const solutionWidth = 4;
const solutionHeight = 5;

let loadedSolutions = {};
let solutionStepsCount = 0;
let currentStep = 1;

window.onload = () => {  
  const fileInputButton = document.getElementById('fileInput');

  fileInputButton.addEventListener('change', function (ev) {

    var reader = new FileReader();

    reader.onload = function(readerEvent) {
      let txtFile = readerEvent.target.result;
      processRawSolution(txtFile);
      solutionStepsCount = Object.keys(loadedSolutions).length;
      
      let nav = document.getElementsByClassName('solutionNavigationInner')[0];
      nav.style.display = "flex";

      document.getElementsByClassName('totalStepsCount')[0].innerHTML = solutionStepsCount;
      document.getElementsByClassName('solutionsSlider')[0].max = solutionStepsCount;

      setManualInputVal(currentStep);
      displayNewStep(currentStep);
    }

    reader.readAsText(fileInputButton.files[0]);
  })
};

function processRawSolution(solutionStr) {
  loadedSolutions = {};
  let split = solutionStr.split('step ');

  split.forEach(line => {
    if(line.length < 2){
      console.log("skip")
      return;
    }
    let lineArgs = line.replaceAll("\r\n", " ")
    let step = parseInt(lineArgs.match(/^[0-9]+/g)[0],);

    let firstSpace = lineArgs.indexOf(' ');

    let rest = lineArgs.substring(firstSpace + 1, lineArgs.length).match(/[^ ]+/g);

    var res = rest.reduce((a, c, i) => {
      return i % solutionWidth === 0 ? a.concat([rest.slice(i, i + solutionWidth).map(el => parseInt(el))]) : a;
    }, []);
    
    loadedSolutions[step] = res;
  });
}

function removeAllGridElements(){
  let grid = document.getElementsByClassName("grid")[0];
  grid.innerHTML = '';
}

function displayNewStep(step) {
  let newStep = parseInt(step);
  currentStep = newStep;
  gridDisplayCurrentStep(currentStep);
  document.getElementsByClassName('currentStep')[0].innerHTML = step;
}

function gridDisplayCurrentStep(currentStep) {
  removeAllGridElements();
  let currentSolution = loadedSolutions[currentStep];

  let grid = document.getElementsByClassName("grid")[0];
  for (let i = 0; i < solutionHeight; i++) {
    for (let j = 0; j < solutionWidth; j++) {
      let cell = currentSolution[i][j];
      let color = '';
      if(cell == 0) {
        color = "white"
      }else if (cell == 1){
        color = "blue";
      } else if(cell === 50){
        color = "red";
      } else if(cell % 2 === 1){
        color = 'green';
      } else if(cell % 2 === 0){
        color = 'orange';
      }

      let div = document.createElement('gridDiv');
      div.className = `${color} ${i}-${j}`;
      grid.appendChild(div);
    }
  }
}

function sliderMoved(value) {
  let newStep = parseInt(value);
  displayNewStep(newStep);
  setManualInputVal(newStep);
}

function setManualInputVal(val) {
 document.getElementsByClassName("manualInput")[0].value = val;
}

function goToStep() {
  let newStep = document.getElementsByClassName("manualInput")[0].value;
  document.getElementsByClassName('sliderContainer')[0].value = newStep;
  displayNewStep(newStep);
}