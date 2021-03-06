﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMM.Data;
using WMM.WPF.Forecast;
using WMM.WPF.Helpers;
using WMM.WPF.MVVM;

namespace WMM.WPF.Goals
{
    public class  GoalYearDetailsViewModel : ObservableObject
    {
        private readonly Goal _goal;
        private readonly IRepository _repository;
        private GoalYearInfo _goalYearInfo;
        private double _average;
        private double _averageDiff;

        public double Limit => _goal.Limit;
        public string Name => _goal.Name;
        public string Description => _goal.Description;

        public string CriteriaString => CreateCriteriaString(_goal);

        public GoalYearInfo GoalYearInfo
        {
            get => _goalYearInfo;
            set => SetValue(ref _goalYearInfo, value);
        }

        public GoalYearDetailsViewModel(Goal goal, IRepository repository)
        {
            _goal = goal;
            _repository = repository;
        }

        public double Average
        {
            get => _average;
            set => SetValue(ref _average,value);
        }

        public double AverageDiff
        {
            get => _averageDiff;
            set => SetValue(ref _averageDiff, value);
        }

        public async Task Initialize()
        {
            var transactions = 
                (await _repository.GetTransactions(DateTime.Today.FirstDayOfMonth().AddMonths(-12), DateTime.Today.FirstDayOfMonth().AddDays(-1), _goal)).OrderBy(x => x.Date).ToList();
            GoalYearInfo = GoalCalculator.CalculateGoalYearInfo(_goal, transactions);
            Average = GoalYearInfo.Average;
            AverageDiff = GoalYearInfo.Average - GoalYearInfo.Limit;
        }

        private string CreateCriteriaString(Goal goal)
        {
            var stringBuilder = new StringBuilder();
            var first = true;
            foreach (var categoryType in goal.CategoryTypeCriteria)
            {
                if (!first)
                    stringBuilder.Append(", ");
                stringBuilder.Append(categoryType.ToCaption());
                first = false;
            }

            foreach (var area in goal.AreaCriteria)
            {
                if (!first)
                    stringBuilder.Append(", ");
                stringBuilder.Append(area);
                first = false;
            }

            foreach (var category in goal.CategoryCriteria)
            {
                if (!first)
                    stringBuilder.Append(", ");
                stringBuilder.Append(category.Name);
                first = false;
            }

            return stringBuilder.ToString();
        }


    }
}
