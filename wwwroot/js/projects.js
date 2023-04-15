'use strict';

$(document).ready(function () {
  const projectCategories = document.querySelectorAll('.list-item');
  const projects = document.querySelectorAll('.portfolioProject');

  const classListSet = createClassListSet(projects);
  const classListArr = [...classListSet];
  setCategoryState(projectCategories, classListArr);
});

function createClassListSet(projectList) {
  const classListSet = new Set();
  projectList.forEach(function (project) {
    Array.from(project.classList).forEach(function (projectName) {
      classListSet.add(projectName);
    });
  });

  return classListSet;
}

function setCategoryState(categories, classListArr) {
  categories.forEach(function (category) {
    const isMatch = classListArr.includes(category.innerHTML.toLowerCase());

    if (isMatch === false && category.innerHTML.toLowerCase() !== 'all') {
      category.disabled = true;
      $(category).css('color', '#e0ebeb');
    } else {
      category.disabled = false;
      $(category).css('color', '#666');
    }
  });
}
