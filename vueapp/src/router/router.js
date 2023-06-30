import {createRouter, /*createWebHistory,*/ createWebHashHistory } from 'vue-router';
import routerGuards from './routerGuards.js';
import {PermissionLevel} from '@/mixins/systemEnums.js';

const routes = [
	{
		path: '/',
		name: 'home',
		component: () => import('@/views/Home.vue'),
	},
	{
		path: '/Login',
		name: 'login',
		component: () => import('@/views/auth/Login.vue'),
	},
	{
		path: '/SignUp',
		name: 'sign-up',
		component: () => import('@/views/auth/SignUp.vue'),
	},
	{
		path: '/Config/Window',
		name: 'config-window',
		component: () => import('@/views/windowConfig/ConfigWindow.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Public.value,
		},
	},
	
	{
		path: '/Management/Tools',
		name: 'management-tools',
		component: () => import('@/views/management/menus/AdminTools.vue'),
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},
	{
		path: '/Management/Users',
		name: 'management-users',
		component: () => import('@/views/management/menus/Users.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},
	{
		path: '/Management/User/:id?',
		name: 'management-user',
		component: () => import('@/views/management/forms/User.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},
	{
		path: '/Management/Brands',
		name: 'management-brands',
		component: () => import('@/views/management/menus/Brands.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},
	{
		path: '/Management/Brand/:id?',
		name: 'management-brand',
		component: () => import('@/views/management/forms/Brand.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},

	{
		path: '/Management/WindowProfiles',
		name: 'management-window-profiles',
		component: () => import('@/views/management/menus/WindowProfiles.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},
	{
		path: '/Management/WindowProfile/:id?',
		name: 'management-window-profile',
		component: () => import('@/views/management/forms/WindowProfile.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},

	{
		path: '/Management/WindowGlassTypes',
		name: 'management-window-glass-types',
		component: () => import('@/views/management/menus/WindowGlassTypes.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},
	{
		path: '/Management/WindowGlassType/:id?',
		name: 'management-window-glass-type',
		component: () => import('@/views/management/forms/WindowGlassType.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},

	{
		path: '/Management/WindowColors',
		name: 'management-window-colors',
		component: () => import('@/views/management/menus/WindowColors.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},
	{
		path: '/Management/WindowColor/:id?',
		name: 'management-window-color',
		component: () => import('@/views/management/forms/WindowColor.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Admin.value,
		},
	},


	{
		path: '/Management/Windows',
		name: 'management-windows',
		component: () => import('@/views/management/menus/Windows.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Client.value,
		},
	},
	{
		path: '/Management/Window/:id?',
		name: 'management-window',
		component: () => import('@/views/management/forms/Window.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Client.value,
		},
	},

	{
		path: '/Management/BudgetWindow/:budgetId/:id?',
		name: 'management-window-budget',
		component: () => import('@/views/management/forms/WindowBudget.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Client.value,
		},
	},

	{
		path: '/Management/ClientBudgetWindow/:clientId/:budgetId/:id?',
		name: 'management-window-client-budget',
		component: () => import('@/views/management/forms/WindowClientBudget.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Technic.value,
		},
	},


	{
		path: '/Management/Budgets/:clientId?',
		name: 'management-budgets',
		component: () => import('@/views/management/menus/Budgets.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Client.value,
		},
	},
	{
		path: '/Management/Budget/:id?/:clientId?',
		name: 'management-budget',
		props: true,
		component: () => import('@/views/management/forms/Budget.vue'),
		meta: {
			role: PermissionLevel.Client.value,
		},
	},



	{
		path: '/Management/Clients',
		name: 'management-clients',
		component: () => import('@/views/management/menus/Clients.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Technic.value,
		},
	},
	{
		path: '/Management/Client/:id?',
		name: 'management-client',
		component: () => import('@/views/management/forms/Client.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Technic.value,
		},
	},

	{
		path: '/Management/ClientBudgets/:clientId?',
		name: 'management-budgets-client',
		component: () => import('@/views/management/menus/BudgetsByClient.vue'),
		props: true,
		meta: {
			role: PermissionLevel.Technic.value,
		},
	},
	{
		path: '/Management/ClientBudget/:id?/:clientId?',
		name: 'management-budget-client',
		props: true,
		component: () => import('@/views/management/forms/BudgetOfClient.vue'),
		meta: {
			role: PermissionLevel.Technic.value,
		},
	},

];

const router = createRouter({
	base: import.meta.env.BASE_URL,
	history: createWebHashHistory(),/*createWebHistory(),*/
	routes,
});

router.beforeEach(routerGuards.beforeEach);

export default router;
