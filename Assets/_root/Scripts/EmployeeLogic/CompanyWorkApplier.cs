using Core;

namespace Scripts.EmployeeLogic
{
    public class CompanyWorkApplier : IExecute
    {
        private readonly Company _company;

        public CompanyWorkApplier(Company company)
        {
            _company = company;
        }

        public void Execute(float deltaTime)
        {
            _company.Update(deltaTime);
        }
    }
}