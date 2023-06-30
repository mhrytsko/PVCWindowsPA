import auth from '@/api/auth';

// Guard de rota para verificar se o utilizador está autenticado e tem nível suficiente para aceder
export default {
	beforeEach(to) {
		if (to.matched.some((record) => !auth.checkUserRole(record.meta.role))) {
			// se a rota requer autenticação, verifica se o usuário está autenticado
			if (
				!auth.isAuthenticated() &&
				// ❗️ Avoid an infinite redirect
				to.name !== 'login'
			) {
				// se o usuário não estiver autenticado, redireciona para a página de login
				return {
					name: 'login',
					query: {redirect: to.fullPath},
				};
			} else {
				// se o utilizador estiver autenticado, permite o acesso à rota
				return {
					name: 'home',
				};
			}
		}

		// se a rota não requer autenticação, permite o acesso à rota
		return true;
	},
};
